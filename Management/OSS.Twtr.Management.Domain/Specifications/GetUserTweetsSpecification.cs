using OSS.Twtr.Domain;
using OSS.Twtr.Management.Domain.Contracts;

namespace OSS.Twtr.Management.Domain.Specifications;

public class GetUserTweetsSpecification : ListSpecification<TweetDto, UserTweetDto>
{
    private readonly UserId _userId;

    public GetUserTweetsSpecification(UserId userId, int page, int count) : base(page, count)
    {
        _userId = userId;
    }
    
    public override IQueryable<UserTweetDto> SatisfyingElementsFrom(IQueryable<TweetDto> queryable)
    {
        return queryable
            .Where(q => q.UserId == _userId.Value)
            .Select(q => new UserTweetDto(q.Id, q.Message, q.PostedOn))
            .OrderByDescending(q => q.PostedOn)
            .Skip((Page-1)*ItemsPerPage)
            .Take(ItemsPerPage);
    }
}