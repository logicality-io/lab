using Microsoft.AspNetCore.Mvc;

namespace SignIn;

[Route("api/{controller}")]
public class HealthController : ControllerBase
{
    public async Task<ActionResult> Get()
    {
        return Ok();
    }
}