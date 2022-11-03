using OSS.Twtr.App.Domain.Aggregates;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Repositories;

public interface ITweetRepository : IRepository<Author, UserId>
{
}