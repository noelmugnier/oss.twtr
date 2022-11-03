using OSS.Twtr.Domain;

namespace OSS.Twtr.Auth.Domain.Events;

public record UserConnected(Guid UserId) : DomainEvent;