using OSS.Twtr.Domain;
using OSS.Twtr.Management.Domain.Contracts;

namespace OSS.Twtr.Management.Domain.Specifications;

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