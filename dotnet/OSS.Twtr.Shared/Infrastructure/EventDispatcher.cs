using Hangfire;
using MediatR;
using OSS.Twtr.Application;
using OSS.Twtr.Domain;

namespace OSS.Twtr.Infrastructure;

public sealed class EventDispatcher : IEventDispatcher
{
    private readonly IBackgroundJobClient _mediator;

    public EventDispatcher(IBackgroundJobClient mediator)
    {
        _mediator = mediator;
    }

    public void Dispatch<T>(T @event) where T : DomainEvent
    {
        var typeArguments = @event.GetType();
        var genericType = typeof(WrappedDomainEvent<>).MakeGenericType(typeArguments);
        var notification = (INotification) Activator.CreateInstance(genericType, @event)!;
        
        _mediator.Enqueue<MediatorHangfireBridge>(bridge => bridge.Publish(typeArguments.Name, notification));
    }

    public void Dispatch<T>(IEnumerable<T> events) where T : DomainEvent
    {
        foreach (var @event in events.OrderBy(e => e.RaisedOn))
            Dispatch(@event);
    }
}