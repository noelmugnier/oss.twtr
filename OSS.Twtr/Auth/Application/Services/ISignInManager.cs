using OSS.Twtr.Common.Core;

namespace OSS.Twtr.Auth.Application.Services;

public interface ISignInManager
{
    Task<Result<Unit>> SignInAsync(LoginUserResult loginResult, bool rememberMe, CancellationToken token);
}