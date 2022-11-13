using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct UpdateProfileCommand(Guid UserId, string? DisplayName, string? Description, string? Job, string?
 Location, string? Url, DateTime? BirthDate) : ICommand<Result<Unit>>;

public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.UserId).NotEqual(Guid.Empty)
            .WithMessage("You must be authenticated to update your profile");
    }
}

internal sealed class UpdateProfileHandler : ICommandHandler<UpdateProfileCommand, Result<Unit>>
{
    private readonly AppDbContext _repository;
    public UpdateProfileHandler(AppDbContext repository) => _repository = repository;

    public async Task<Result<Unit>> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var user = await _repository.Set<User>().SingleOrDefaultAsync(u => u.Id == UserId.From(request.UserId), ct);
        if(user == null)
            return new Result<Unit>(new Error("User profile not found"));
        
        user.DisplayName = request.DisplayName;
        user.Description = request.Description;
        user.Job = request.Job;
        user.Location = request.Location;
        user.Url = request.Url;
        user.BirthDate = request.BirthDate;
        
        var results= await _repository.SaveChangesAsync(ct);
        return results > 0 ? new Result<Unit>(Unit.Value):new Result<Unit>(new Error("Failed to update profile"));
    }
}