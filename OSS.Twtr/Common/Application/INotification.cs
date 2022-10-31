using MediatR;
using OSS.Twtr.Common.Domain;

namespace OSS.Twtr.Common.Application;

public record WrappedDomainEvent<T>(T DomainEvent) : INotification where T: DomainEvent;