using MediatR;

namespace OSS.Twtr.Common.Application;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}

public interface IQueryDispatcher
{
    Task<TResponse> Execute<TResponse>(IQuery<TResponse> request, CancellationToken token = default);
}