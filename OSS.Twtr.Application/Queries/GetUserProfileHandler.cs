using FluentValidation;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Domain.Contracts;
using OSS.Twtr.Domain.Repositories;

namespace OSS.Twtr.Management.Application.Queries;

public class GetUserProfileHandler : IQueryHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUserRepository _repository;

    public GetUserProfileHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var user = await _repository.GetUserProfile((UserId)request.UserId, ct);
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