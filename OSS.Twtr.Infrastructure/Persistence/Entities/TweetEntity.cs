using LinqToDB.Mapping;

namespace OSS.Twtr.Management.Infrastructure.Persistence;

[Table("Tweets")]
public class TweetEntity
{
    [Column] public Guid Id { get; init; }
    [Column] public string Message { get; set; }
    [Column] public Guid UserId { get; set; }
    [Column] public DateTime PostedOn { get; set; }
}