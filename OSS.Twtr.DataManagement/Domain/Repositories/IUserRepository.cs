using OSS.Twtr.Domain.Ids;

namespace OSS.Twtr.TweetManagement.Domain;

public interface IUserRepository
{
    Task<User> Get(UserId id, CancellationToken token);
    Task<UserProfileDto> GetUserProfile(UserId id, CancellationToken token);
    void Add(User entity);
    void Delete(User entity);
}