using MediatR;
using OSS.Twtr.Core;

namespace OSS.Twtr.Application;

public record WrappedDomainEvent<T>(T DomainEvent) : INotification where T: DomainEvent;