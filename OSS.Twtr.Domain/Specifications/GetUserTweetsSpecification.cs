using OSS.Twtr.Core;
using OSS.Twtr.Domain.Contracts;

namespace OSS.Twtr.Domain.Specifications;

public class GetUserTweetsSpecification : Specification<TweetDto, UserTweetDto>
{
    private readonly UserId _userId;
    private readonly int _page;
    private readonly int _count;

    public GetUserTweetsSpecification(UserId userId, int page, int count)
    {
        _userId = userId;
        _page = page;
        _count = count;
    }
    
    public override IQueryable<UserTweetDto> SatisfyingElementsFrom(IQueryable<TweetDto> queryable)
    {
        return queryable
            .Where(q => q.UserId == _userId)
            .Select(q => new UserTweetDto(q.Id, q.Message, q.PostedOn))
            .OrderByDescending(q => q.PostedOn)
            .Skip((_page-1)*_count)
            .Take(_count);
    }
}