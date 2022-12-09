using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Repositories;

public interface ITweetRepository : IRepository<Tweet, TweetId>
{
}