using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Logicality.ExampleGateway.SignIn;

public class AccountController : Controller
{
    [HttpGet]
    public async Task<ActionResult> Login(string returnUrl = "/")
    {
        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = returnUrl,
        };
        return Challenge(authenticationProperties, "oidc");
    }
}