using Microsoft.EntityFrameworkCore;
using PostService.Models;

namespace PostService.DataAccess;

public class GenericEfRepository<TModel> : IGenericRepository<TModel> where TModel : ModelBase
{
    private readonly DbContext context;

    public GenericEfRepository(DbContext context)
    {
        this.context = context;
    }

    public List<TModel> GetList()
    {
        return context.Set<TModel>().ToList();
    }

    public TModel? GetById(int id)
    {
        return context.Set<TModel>().FirstOrDefault(m => m.Id == id);
    }

    public int Create(TModel model)
    {
        context.Set<TModel>().Add(model);
        context.SaveChanges();
        return model.Id;
    }

    public void Update(TModel model)
    {
        context.Set<TModel>().Update(model);
        context.SaveChanges();
    }

    public int Delete(int id)
    {
        var model = context.Set<TModel>().Find(id);
        if (model == null)
            return 0;

        context.Set<TModel>().Remove(model);
        return context.SaveChanges();
    }
}
