using OSS.Twtr.Core;

namespace OSS.Twtr.App.Application;

internal interface IOpenAIGenerator
{
    Task<Result<AiReplyDto?>> GenerateReplyFromCorrespondingAiUser(string? message);
}