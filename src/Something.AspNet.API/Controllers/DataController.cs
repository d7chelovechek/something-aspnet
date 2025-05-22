using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Something.AspNet.API.Controllers;

[ApiController]
[Route("data")]
[JwtAuthorize]
public class DataController : ControllerBase
{
    [HttpGet("values")]
    public IEnumerable<int> GetValues()
    {
        return Enumerable.Range(0, 10);
    }
}