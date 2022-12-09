using OSS.Twtr.Domain;

namespace OSS.Twtr.App.Domain.Events;

public record UserUnblockBy(Guid UnblockedUserId, Guid ByUserId) : DomainEvent;