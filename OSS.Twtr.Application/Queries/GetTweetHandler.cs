using FluentValidation;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Domain.Contracts;
using OSS.Twtr.Domain.Repositories;
using OSS.Twtr.Domain.Specifications;

namespace OSS.Twtr.Management.Application.Queries;

public class GetTweetHandler : IQueryHandler<GetTweetQuery, Result<TweetDto>>
{
    private readonly ITweetRepository _tweetRepository;

    public GetTweetHandler(ITweetRepository tweetRepository)
    {
        _tweetRepository = tweetRepository;
    }

    public async Task<Result<TweetDto>> Handle(GetTweetQuery request, CancellationToken ct)
    {
        var tweet = await _tweetRepository.Get(new GetTweetById(TweetId.From(request.TweetId)), ct);
        return new Result<TweetDto>(tweet);
    }
}

public record struct GetTweetQuery(Guid TweetId) : IQuery<Result<TweetDto>>;

public sealed class GetTweetValidator : AbstractValidator<GetTweetQuery>
{
    public GetTweetValidator()
    {
        RuleFor(x => x.TweetId).NotEqual(Guid.Empty);
    }
}