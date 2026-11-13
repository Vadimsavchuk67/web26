using PostService.CommonTypes;
using PostService.DataAccess;
using PostService.Models;

namespace PostService.BusinessLogic;

public class PostingService : IPostingService
{
    private readonly IPostingRepository repository;

    public PostingService(IPostingRepository repository)
    {
        this.repository = repository;
    }

    public Posting Create(Posting newPosting)
    {
        newPosting.CreatedAt = DateTime.UtcNow;
        float rate = 10;
        if (newPosting.DeliveryType == DeliveryType.Courier)
            rate = 20;
        else if (newPosting.DeliveryType == DeliveryType.ExpressCourier)
            rate = 30;

        newPosting.Price = newPosting.Weight * rate;
        newPosting.Id = repository.Create(newPosting);
        return newPosting;
    }

    public List<Posting> GetAll()
    {
        return repository.GetList();
    }

    public Posting? Find(int postingId)
    {
        return repository.GetById(postingId);
    }

    public Posting? Update(Posting updated)
    {
        var posting = repository.GetById(updated.Id);
        if (posting == null)
            return null;

        posting.From = updated.From;
        posting.To = updated.To;
        posting.Content = updated.Content;
        posting.DeliveryType = updated.DeliveryType;
        posting.Weight = updated.Weight;
        posting.Width = updated.Width;
        posting.Height = updated.Height;
        posting.Depth = updated.Depth;
        posting.Value = updated.Value;
        posting.Price = updated.Price;

        repository.Update(posting);
        return posting;
    }

    public int Delete(int postingId)
    {
        return repository.Delete(postingId);
    }
}
