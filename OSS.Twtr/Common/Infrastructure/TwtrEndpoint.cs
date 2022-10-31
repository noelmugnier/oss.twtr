using FastEndpoints;
using FluentValidation;
using OSS.Twtr.Common.Core;

namespace OSS.Twtr.Common.Infrastructure;

public abstract class TwtrEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse> 
    where TRequest : notnull, new() where TResponse : notnull
{
    protected Task SendResultErrorsAsync<T>(Result<T> result, CancellationToken token)
    {
        if (result.Success)
            throw new InvalidOperationException("Cannot return Result<T> errors because it's in success state.");
        
        foreach (var error in result.Errors)
            AddError(error.Message, error.ErrorCode.ToString("G"), (Severity) error.ErrorSeverity);
        
        return SendErrorsAsync(cancellation: token);
    }
    
    protected Task SendResultErrorsAsync(IEnumerable<Error> errors, CancellationToken token)
    {
        foreach (var error in errors)
            AddError(error.Message, error.ErrorCode.ToString("G"), (Severity) error.ErrorSeverity);
        
        return SendErrorsAsync(cancellation: token);
    }
}