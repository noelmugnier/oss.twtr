using System.ComponentModel;
using MediatR;

namespace OSS.Twtr.Infrastructure;

public class MediatorHangfireBridge
{
    private readonly IMediator _mediator;

    public MediatorHangfireBridge(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Send(IRequest command)
    {
        await _mediator.Send(command);
    }

    [DisplayName("{0}")]
    public async Task Send(string jobName, IRequest command)
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