using MediatR;
using OSS.Twtr.Domain;

namespace OSS.Twtr.Application;

public record WrappedDomainEvent<T>(T DomainEvent) : INotification where T: DomainEvent;