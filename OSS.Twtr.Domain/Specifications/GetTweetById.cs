using OSS.Twtr.Core;
using OSS.Twtr.Domain.Contracts;

namespace OSS.Twtr.Domain.Specifications;

public class GetTweetById : Specification<TweetDto>
{
    private readonly TweetId _tweetId;

    public GetTweetById(TweetId tweetId)
    {
        _tweetId = tweetId;
    }

    public override IQueryable<TweetDto> SatisfyingElementsFrom(IQueryable<TweetDto> candidates)
    {
        return candidates.Where(q => q.Id == _tweetId.Value);
    }
}