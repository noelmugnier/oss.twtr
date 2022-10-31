using MediatR;
using OSS.Twtr.Common.Application;

namespace OSS.Twtr.Common.Infrastructure;

public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IMediator _mediator;

    public QueryDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<TResponse> Execute<TResponse>(IQuery<TResponse> request, CancellationToken token = default)
    {
        return _mediator.Send(request, token);
    }
}