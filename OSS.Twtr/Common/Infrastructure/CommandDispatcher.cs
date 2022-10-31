using MediatR;
using OSS.Twtr.Common.Application;

namespace OSS.Twtr.Common.Infrastructure;

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IMediator _mediator;

    public CommandDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<TResponse> Execute<TResponse>(ICommand<TResponse> request, CancellationToken token = default)
    {
        return _mediator.Send(request, token);
    }
}