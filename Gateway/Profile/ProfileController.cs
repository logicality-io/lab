using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logicality.ExampleGateway.Profile;

[Authorize("RequireInteractiveUser")]
public class ProfileController : ControllerBase
{
    // GET
    [HttpGet("profile")]
    public IActionResult Get()
    {
        var userClaims = HttpContext.User.Claims.ToArray();
        var serializedClaims = userClaims.Select(x =>
            new
            {
                Type  = x.Type,
                Value = x.Value
            });
        return Ok(serializedClaims);
    }
}