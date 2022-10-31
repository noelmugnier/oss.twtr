namespace OSS.Twtr.Common.Domain;

public interface DomainObject{}
public interface IdentifiableId{}

public abstract class IdentifiableDomainObject<TId> : DomainObject 
    where TId : IdentifiableId
{
    protected IdentifiableDomainObject(TId id) => Id = id; 
    public TId Id { get; }
}