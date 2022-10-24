using MediatR;
using OSS.Twtr.Application;
using OSS.Twtr.Domain;
using OSS.Twtr.Management.Application.Commands;

namespace OSS.Twtr.Management.Application.Events;

public class AccountCreatedHandler : IEventHandler<AccountCreated>
{
    private readonly IMediator _mediator;

    public AccountCreatedHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Handle(WrappedDomainEvent<AccountCreated> notification, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateUserCommand(notification.DomainEvent.AccountId.Value, notification.DomainEvent.UserName, notification.DomainEvent.RaisedOn), cancellationToken);
    }
}