using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using OSS.Twtr.AccountManagement.Application;
using OSS.Twtr.AccountManagement.Infrastructure;
using OSS.Twtr.Infrastructure.Services;
using Xunit;

namespace OSS.Twtr.Integration.Tests;

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
    
    [Fact]
    public async Task Fail()
    {
        await _handler.Handle(new CreateAccountCommand("test", "password", "paord"), CancellationToken.None);
        
        var account = await _appDbConnection.Accounts.FirstAsync();
        Assert.NotNull(account);
    }
}