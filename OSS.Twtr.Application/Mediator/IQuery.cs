using MediatR;
using OSS.Twtr.Core;

namespace OSS.Twtr.Application;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}

public interface IEventHandler<T> : INotificationHandler<WrappedDomainEvent<T>> 
    where T:DomainEvent
{
}