using OSS.Twtr.Common.Domain;
using OSS.Twtr.Tweet.Domain.Aggregates;
using OSS.Twtr.Tweet.Domain.ValueObjects;

namespace OSS.Twtr.Tweet.Domain.Repositories;

public interface ITweetRepository : IRepository<Author, UserId>
{
}