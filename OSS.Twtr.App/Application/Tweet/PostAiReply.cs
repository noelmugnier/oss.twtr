using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenAI;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels.ResponseModels;
using OSS.Twtr.App.Domain.Entities;
using OSS.Twtr.App.Domain.Repositories;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.App.Infrastructure;
using OSS.Twtr.Application;
using OSS.Twtr.Core;
using Error = OSS.Twtr.Core.Error;

namespace OSS.Twtr.App.Application;

public record struct GenerateAiReplyCommand(Guid TweetId) : ICommand<Result<AiReplyDto>>;

public record struct AiReplyDto(string Message, Guid ReplierId);

public sealed class PostAiReplyValidator : AbstractValidator<GenerateAiReplyCommand>
{
    public PostAiReplyValidator()
    {
        RuleFor(x => x.TweetId).NotEqual(Guid.Empty)
            .WithMessage("You must specify the tweet to reply to.");
    }
}

internal sealed class PostAiReplyHandler : ICommandHandler<GenerateAiReplyCommand, Result<AiReplyDto>>
{
    private readonly AppDbContext _repository;
    private readonly IOpenAIGenerator _openAiGenerator;
    private readonly ICommandDispatcher _commandDispatcher;

    public PostAiReplyHandler(AppDbContext repository, IOpenAIGenerator openAiGenerator, ICommandDispatcher commandDispatcher)
    {
        _repository = repository;
        _openAiGenerator = openAiGenerator;
        _commandDispatcher = commandDispatcher;
    }

    public async Task<Result<AiReplyDto>> Handle(GenerateAiReplyCommand request, CancellationToken ct)
    {
        try
        {
            var tweet = await _repository.Set<Tweet>().SingleOrDefaultAsync(t => t.Id == TweetId.From(request.TweetId), ct);
            if (tweet == null)
                return new Result<AiReplyDto>(new Error("Tweet not found."));

            var result = await _openAiGenerator.GenerateReplyFromCorrespondingAiUser(tweet.Message);
            if (!result.Success)
                return new Result<AiReplyDto>(result.Errors);
            
            return result.Data == null 
                ? new Result<AiReplyDto>(new Error("No reply generated.")) 
                : new Result<AiReplyDto>(result.Data.Value);
        }
        catch (Exception e)
        {
            return new Result<AiReplyDto>(e);
        }
    }
}