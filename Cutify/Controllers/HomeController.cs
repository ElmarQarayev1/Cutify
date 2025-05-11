using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cutify.Models;
using Microsoft.AspNetCore.Authorization;

namespace Cutify.Controllers;

public class HomeController : Controller
{
    public HomeController()
    {
        
    }
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

 
}

