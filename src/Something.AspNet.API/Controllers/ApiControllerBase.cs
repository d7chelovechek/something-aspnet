using Microsoft.AspNetCore.Mvc;
using Something.AspNet.API.Models;

namespace Something.AspNet.API.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected SessionPrincipal Session => (SessionPrincipal)User;
}