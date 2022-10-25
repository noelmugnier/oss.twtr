using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using OSS.Twtr.Core;
using OSS.Twtr.Identity.Application.Commands;
using Xunit;

namespace OSS.Twtr.Identity.Tests;

public class CreateAccountShould
{
    private readonly CreateAccountHandler _handler;
    private readonly IdentityDbContext<IdentityUser> _context;

    public CreateAccountShould()
    {
        _context = new DbContext(new DbContextOptionsBuilder<DbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        _handler = new CreateAccountHandler(
            new UserManager<IdentityUser>(
                new UserStore<IdentityUser>(_context, new IdentityErrorDescriber()),
                new OptionsWrapper<IdentityOptions>(new IdentityOptions()),
                new PasswordHasher<IdentityUser>(
                    new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())),
                new List<IUserValidator<IdentityUser>>(),
                new List<IPasswordValidator<IdentityUser>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new ServiceCollection().BuildServiceProvider(),
                new NullLogger<UserManager<IdentityUser>>()),
            Substitute.For<IEventDispatcher>());
    }

    [Fact]
    public async Task Succeed()
    {
        await _handler.Handle(new CreateAccountCommand("test", "password", "password"), CancellationToken.None);

        var user = await _context.Users.FirstAsync();
        Assert.NotNull(user);
    }
}