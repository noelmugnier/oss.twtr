using FluentValidation;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using OSS.Twtr.Domain;
using OSS.Twtr.Management.Domain;
using OSS.Twtr.Management.Domain.Contracts;

namespace OSS.Twtr.Management.Application.Commands;

public class CreateTweetHandler : ICommandHandler<CreateTweetCommand, Result<TweetDto>>
{
    private readonly IDataDbContext _db;

    public CreateTweetHandler(IDataDbContext db)
    {
        _db = db;
    }

    public async Task<Result<TweetDto>> Handle(CreateTweetCommand request, CancellationToken ct)
    {
        var user = await _db.Users.Get(UserId.From(request.UserId), ct);
        var tweet = new Tweet(request.Message, user);
        
        _db.Tweets.Add(tweet);
        var result = await _db.SaveChanges(ct);

        var dto = await _db.Tweets.Get(tweet.Id, ct);
        
        return result.On<Result<TweetDto>>(success => new(dto), errors => new(errors));
    }
}

public record struct CreateTweetCommand(Guid UserId, string Message) : ICommand<Result<TweetDto>>;

public sealed class CreateTweetValidator : AbstractValidator<CreateTweetCommand>
{
    public CreateTweetValidator()
    {
        RuleFor(x => x.Message).NotEmpty();
        RuleFor(x => x.UserId).NotEqual(Guid.Empty);
    }
}