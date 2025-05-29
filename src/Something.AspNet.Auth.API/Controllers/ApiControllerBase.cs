using Microsoft.AspNetCore.Mvc;
using Something.AspNet.Auth.API.Models;

namespace Something.AspNet.Auth.API.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected SessionPrincipal Session => (SessionPrincipal)User;
}