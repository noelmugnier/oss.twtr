using OSS.Twtr.Domain;

namespace OSS.Twtr.Auth.Domain.Events;

public record UserCreated(Guid UserId) : DomainEvent;