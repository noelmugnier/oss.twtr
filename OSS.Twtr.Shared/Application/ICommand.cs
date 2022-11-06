using MediatR;

namespace OSS.Twtr.Application;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface IEventHandler<T> : INotificationHandler<T>
    where T : INotification
{
}


public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}

public interface ICommandDispatcher
{
    Task<TResponse> Execute<TResponse>(ICommand<TResponse> request, CancellationToken token = default);
}