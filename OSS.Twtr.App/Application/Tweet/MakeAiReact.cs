using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenAI;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

public record struct MakeAiReactCommand(Guid TweetId) : ICommand<Result<Unit>>;

public sealed class MakeAiReactValidator : AbstractValidator<MakeAiReactCommand>
{
    public MakeAiReactValidator()
    {
        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify the tweet to unpin");
    }
}

internal sealed class MakeAiReactHandler : ICommandHandler<MakeAiReactCommand, Result<Unit>>
{
    private readonly IOpenAIService _openAiClient;
    private readonly AppDbContext _repository;
    public MakeAiReactHandler(IOpenAIService openAiClient, AppDbContext repository)
    {
        _openAiClient = openAiClient;
        _repository = repository;
    }

    public async Task<Result<Unit>> Handle(MakeAiReactCommand request, CancellationToken ct)
    {
        var tweet = await _repository.Set<Tweet>().SingleOrDefaultAsync(t => t.Id == TweetId.From(request.TweetId), ct);
        if (tweet == null)
            return new Result<Unit>(Unit.Value);
        
        var sentimentResult = await _openAiClient.Completions.CreateCompletion(
            new CompletionCreateRequest
            {
                Prompt =
                    $"Decide whether a Tweet's sentiment is positive, neutral or negative\n\nTweet: {tweet.Message}\nSentiment:",
                MaxTokens = 5,
                Stop = "Sentiment:"
            }, Models.TextCurieV1);

        var results = await _openAiClient.Completions.CreateCompletion(new CompletionCreateRequest
        {
            Prompt = tweet.Message,
            MaxTokens = 50,
        }, Models.TextDavinciV2);

        var userIdToReply = UserId.None;
        var sentiment = sentimentResult.Choices.First().Text.Trim().ToLowerInvariant();
        switch (sentiment)
        {
            case "positive":
                userIdToReply = Users.PositiveAi;
                break;
            case "neutral":
                userIdToReply = Users.NeutralAi;
                break;
            case "negative":
                userIdToReply = Users.NegativeAi;
                break;
        }

        var reply = tweet.Reply(results.Choices.First().Text.Trim(), userIdToReply);
        await _repository.AddAsync(reply, ct);
        await _repository.SaveChangesAsync(ct);
        
        return new Result<Unit>(Unit.Value);
    }
}

internal class Users
{
    public static UserId PositiveAi => UserId.From(Guid.Parse("26B1C77F-12E2-4B94-AA6C-E6FFF86E09F3"));
    public static UserId NeutralAi => UserId.From(Guid.Parse("D3910D3E-6421-407C-9244-706A550545E5"));
    public static UserId NegativeAi => UserId.From(Guid.Parse("E9D62AA9-E3CE-4D8D-961D-3095F321ABCF"));
}