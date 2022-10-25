using OSS.Twtr.Core;
using OSS.Twtr.Domain.Contracts;

namespace OSS.Twtr.Domain.Repositories;

public interface IUserRepository
{
    Task<User> Get(UserId id, CancellationToken token);
    Task<UserProfileDto> GetUserProfile(UserId id, CancellationToken token);
    void Add(User entity);
    void Delete(User entity);
}