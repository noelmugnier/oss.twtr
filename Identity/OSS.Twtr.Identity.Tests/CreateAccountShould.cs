using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using OSS.Twtr.Identity.Application.Commands;
using OSS.Twtr.Identity.Infrastructure.Persistence;
using OSS.Twtr.Identity.Infrastructure.Services;
using OSS.Twtr.Infrastructure;
using Xunit;

namespace OSS.Twtr.Identity.Tests;

public class CreateAccountShould
{
    private readonly CreateAccountHandler _handler;
    private readonly IdentityDbConnection _appDbConnection;
    
    public CreateAccountShould()
    {
        _appDbConnection = new IdentityDbConnection("Data Source=:memory:;Version=3;New=True;");
        _handler = new CreateAccountHandler(new Hasher(Substitute.For<IPasswordHasher<object>>()), new IdentityDbContext(_appDbConnection, new EventDispatcher(Substitute.For<IMediator>())));
    }
    
    [Fact]
    public async Task Succeed()
    {
        await _handler.Handle(new CreateAccountCommand("test", "password", "password"), CancellationToken.None);
        
        var account = await _appDbConnection.Accounts.FirstAsync();
        Assert.NotNull(account);
    }
}