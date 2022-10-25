using FluentValidation;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Domain.Contracts;
using OSS.Twtr.Domain.Repositories;
using OSS.Twtr.Domain.Specifications;

namespace OSS.Twtr.Management.Application.Queries;

public class GetUserTweetsHandler : IQueryHandler<GetUserTweetsQuery, Result<IEnumerable<UserTweetDto>>>
{
    private readonly ITweetRepository _tweetRepository;

    public GetUserTweetsHandler(ITweetRepository tweetRepository)
    {
        _tweetRepository = tweetRepository;
    }

    public async Task<Result<IEnumerable<UserTweetDto>>> Handle(GetUserTweetsQuery request, CancellationToken ct)
    {
        var tweets = await _tweetRepository.GetAll(new GetUserTweetsSpecification((UserId)request.UserId, request.Page, request.Count), ct);
        return new Result<IEnumerable<UserTweetDto>>(tweets);
    }
}

public record struct GetUserTweetsQuery(Guid UserId, int Page, int Count) : IQuery<Result<IEnumerable<UserTweetDto>>>;

public sealed class GetUserTweetsValidator : AbstractValidator<GetUserTweetsQuery>
{
    public GetUserTweetsValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty);
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Count).GreaterThanOrEqualTo(10);
    }
}