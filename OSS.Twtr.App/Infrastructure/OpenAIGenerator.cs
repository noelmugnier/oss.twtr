using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OSS.Twtr.App.Domain.ValueObjects;
using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

internal sealed class OpenAIGenerator : IOpenAIGenerator
{
    private readonly IOpenAIService _service;

    public OpenAIGenerator(IOpenAIService service)
    {
        _service = service;
    }
    
    public async Task<Result<AiReplyDto?>> GenerateReplyFromCorrespondingAiUser(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return new Result<AiReplyDto?>((AiReplyDto?)null);

        try
        {
            var sentimentResult = await _service.Completions.CreateCompletion(
                new CompletionCreateRequest
                {
                    Prompt =
                        $"Decide whether a Tweet's sentiment is positive, neutral or negative\n\nTweet: {message}\n\nSentiment:",
                    MaxTokens = 5,
                    Stop = "Tweet:,Sentiment:"
                }, Models.TextBabbageV1);

            var sentiment = sentimentResult.Choices.First().Text.Trim().ToLowerInvariant();
            var results = await _service.Completions.CreateCompletion(new CompletionCreateRequest
            {
                Prompt =
                    $"The following is a conversation with an AI. The AI answers are tainted with {sentiment} sentiment.\n\nHuman: {message}\n\nAI:",
                MaxTokens = 50,
                Stop = "Human:,AI:"
            }, Models.TextDavinciV2);

            var userIdToReply = GetAiUserIdToReply(sentiment);
            return new Result<AiReplyDto?>(new AiReplyDto(results.Choices.First().Text.Trim(), userIdToReply.Value));
        }
        catch (Exception e)
        {
            return new Result<AiReplyDto?>(e);
        }
    }

    private static UserId GetAiUserIdToReply(string sentiment)
    {
        var userIdToReply = UserId.None;
        switch (sentiment)
        {
            case "positive":
                userIdToReply = AiUsers.Positive;
                break;
            case "neutral":
                userIdToReply = AiUsers.Neutral;
                break;
            case "negative":
                userIdToReply = AiUsers.Negative;
                break;
        }

        return userIdToReply;
    }
}