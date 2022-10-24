using OSS.Twtr.Domain;
using OSS.Twtr.Management.Domain.Contracts;

namespace OSS.Twtr.Management.Domain.Repositories;

public interface IUserRepository
{
    Task<User> Get(UserId id, CancellationToken token);
    Task<UserProfileDto> GetUserProfile(UserId id, CancellationToken token);
    void Add(User entity);
    void Delete(User entity);
}