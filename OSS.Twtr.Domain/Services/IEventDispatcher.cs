namespace OSS.Twtr.Core;

public interface IEventDispatcher
{
    void Dispatch<T>(T @event) where T : DomainEvent;

    void Dispatch<T>(IEnumerable<T> events) where T : DomainEvent;
}