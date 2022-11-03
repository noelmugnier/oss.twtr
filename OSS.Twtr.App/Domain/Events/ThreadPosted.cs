using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record ThreadPosted(Guid ThreadId, Guid ByUserId) : DomainEvent;