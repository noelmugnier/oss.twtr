using System.ComponentModel;
using MediatR;
using OSS.Twtr.Application;
using OSS.Twtr.Domain;

namespace OSS.Twtr.Infrastructure;

public class MediatorHangfireBridge
{
    private readonly IMediator _mediator;

    public MediatorHangfireBridge(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Execute<TRequest>(ICommand<TRequest> command)
    {
        await _mediator.Send(command);
    }

    [DisplayName("{0}")]
    public async Task Execute<TRequest>(string jobName, ICommand<TRequest> command)
    {
        await _mediator.Send(command);
    }

    public async Task Publish(INotification command)
    {
        await _mediator.Publish(command);
    }

    [DisplayName("{0}")]
    public async Task Publish(string jobName, INotification command)
    {
        await _mediator.Publish(command);
    }
}