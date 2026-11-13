using Microsoft.EntityFrameworkCore;
using PostService.DataAccess;
using PostService.Dtos;
using PostService.Mappings;
using PostService.BusinessLogic;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5211", "http://localhost:3000");
builder.Environment.WebRootPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "../../клієнт"));
builder.Environment.WebRootFileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(builder.Environment.WebRootPath);
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddScoped<IPostingService, PostingService>();
builder.Services.AddScoped<IPostingRepository, PostingRepository>();
builder.Services.AddDbContext<PostingDbContext>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
var app = builder.Build();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PostingDbContext>();
    context.Database.EnsureCreated();
    var service = scope.ServiceProvider.GetRequiredService<IPostingService>();
    if (service.GetAll().Count == 0)
    {
        service.Create(new PostService.Models.Posting { From = "Рівне", To = "Луцьк", Content = "Книга", Weight = 2, Width = 10, Height = 15, Depth = 20, Value = 300 });
        service.Create(new PostService.Models.Posting { From = "Київ", To = "Львів", Content = "Одяг", DeliveryType = PostService.CommonTypes.DeliveryType.Courier, Weight = 3, Width = 25, Height = 15, Depth = 30, Value = 1000 });
        service.Create(new PostService.Models.Posting { From = "Одеса", To = "Дніпро", Content = "Документи", DeliveryType = PostService.CommonTypes.DeliveryType.ExpressCourier, Weight = 1, Width = 20, Height = 2, Depth = 30, Value = 100 });
    }
}



app.MapGet("/postings", (IPostingService service) =>
{
    var postings = service.GetAll();
    return Results.Ok(postings.Select(PostingMapper.ToPostingGetDto).ToList());
});

app.MapGet("/postings/{id:int}", (int id, IPostingService service) =>
{
    var posting = service.Find(id);
    if (posting == null)
        return Results.NotFound();

    return Results.Ok(PostingMapper.ToPostingGetDto(posting));
});

app.MapPost("/postings", (PostingPostDto dto, IPostingService service) =>
{
    var posting = PostingMapper.ToPosting(dto);
    var errors = Validate(posting);
    if (errors.Count > 0) return Results.ValidationProblem(errors);
    var saved = service.Create(posting);
    var result = PostingMapper.ToPostingGetDto(saved);
    return Results.Created($"/postings/{result.Id}", result);
});

app.MapPut("/postings/{id:int}", (int id, PostingPutDto dto, IPostingService service) =>
{
    if (id != dto.Id)
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["Id"] = new[] { "Номер у запиті не збігається з номером відправлення." } });

    var posting = PostingMapper.ToPosting(dto);
    var errors = Validate(posting);
    if (!float.IsFinite(posting.Price) || posting.Price < 0) errors["Price"] = new[] { "Вартість не може бути від’ємною." };
    if (errors.Count > 0) return Results.ValidationProblem(errors);
    var updated = service.Update(posting);
    if (updated == null)
        return Results.NotFound();

    return Results.Ok(PostingMapper.ToPostingGetDto(updated));
});

app.MapDelete("/postings/{id:int}", (int id, IPostingService service) =>
{
    var result = service.Delete(id);
    if (result == 0)
        return Results.NotFound();

    return Results.NoContent();
});

app.Run();

static Dictionary<string, string[]> Validate(PostService.Models.Posting p)
{
    var errors = new Dictionary<string, string[]>();
    if (string.IsNullOrWhiteSpace(p.From)) errors["From"] = new[] { "Вкажіть відправника." };
    if (string.IsNullOrWhiteSpace(p.To)) errors["To"] = new[] { "Вкажіть отримувача." };
    if (string.IsNullOrWhiteSpace(p.Content)) errors["Content"] = new[] { "Вкажіть вміст." };
    if (!Enum.IsDefined(p.DeliveryType)) errors["DeliveryType"] = new[] { "Невідомий тип доставки." };
    if (!float.IsFinite(p.Weight) || p.Weight <= 0) errors["Weight"] = new[] { "Вага має бути більшою за 0." };
    if (p.Width <= 0 || p.Height <= 0 || p.Depth <= 0) errors["Size"] = new[] { "Розміри мають бути більшими за 0." };
    if (!float.IsFinite(p.Value) || p.Value < 0) errors["Value"] = new[] { "Цінність не може бути від’ємною." };
    return errors;
}





