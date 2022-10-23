using FastEndpoints;
using FluentValidation;
using Mapster;
using MediatR;
using OSS.Twtr.AccountManagement.Application;

namespace OSS.Twtr.AccountManagement.Infrastructure;

public class CreateAccountEndpoint : Endpoint<CreateAccountRequest, CreateAccountResponse>
{
    private readonly IMediator _mediator;

    public CreateAccountEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateAccountRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(req.Adapt<CreateAccountCommand>(), ct);
        await result.On(success => SendAsync(new CreateAccountResponse(success.Id.Value, success.UserName), cancellation: ct), errors =>
        {
            foreach (var error in errors)
                AddError(error.Message, error.ErrorCode, (Severity)error.Severity);

            return SendErrorsAsync(cancellation: ct);
        });
    }
}

public record struct CreateAccountRequest(string Username, string Password, string ConfirmPassword);
public record struct CreateAccountResponse(Guid Id, string Username);