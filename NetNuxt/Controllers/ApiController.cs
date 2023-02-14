using Microsoft.AspNetCore.Mvc;
using NetNuxt.Middlewares;

namespace NetNuxt.Controllers;

public class ApiController : Controller, CallAPI
{
    public string post()
    {
        return "";
    }
}