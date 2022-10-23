using FluentValidation;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Domain.Ids;
using OSS.Twtr.TweetManagement.Domain;

namespace OSS.Twtr.TweetManagement.Application;

public class GetUserProfileHandler : IQueryHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUserRepository _repository;

    public GetUserProfileHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var user = await _repository.GetUserProfile(UserId.From(request.UserId), ct);
        return new Result<UserProfileDto>(user);
    }
}

public record struct GetUserProfileQuery(Guid UserId) : IQuery<Result<UserProfileDto>>;

public sealed class GetUserProfileValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty);
    }
}