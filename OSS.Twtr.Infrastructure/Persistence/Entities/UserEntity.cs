using LinqToDB.Mapping;

namespace OSS.Twtr.Management.Infrastructure.Persistence;

[Table("Users")]
public class UserEntity
{
    [Column] public Guid Id { get; init; }
    [Column] public string UserName { get; init; }
    [Column] public string DisplayName { get; init; }
    [Column] public DateTime MemberSince { get; init; }
}