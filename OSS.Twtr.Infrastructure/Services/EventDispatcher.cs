﻿using MediatR;
using OSS.Twtr.Application;
using OSS.Twtr.Domain;
using OSS.Twtr.Domain.Services;

namespace OSS.Twtr.Infrastructure.Services;

public class EventDispatcher : IEventDispatcher
{
    private readonly IMediator _mediator;

    public EventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void Dispatch<T>(T @event) where T : DomainEvent
    {
        var genericType = typeof(WrappedDomainEvent<>).MakeGenericType(@event.GetType());
        var notification = (INotification) Activator.CreateInstance(genericType, @event)!;
        _mediator.Publish(notification);
    }

    public void Dispatch<T>(IEnumerable<T> events) where T : DomainEvent
    {
        foreach (var @event in events.OrderBy(e => e.RaisedOn))
            Dispatch(@event);
    }
}