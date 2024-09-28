using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace showcase;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    [HttpGet("/AuthViews/adminview.html")]
    public IActionResult AdminView()
    {
        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "AuthViews/adminview.html");
        return PhysicalFile(filepath, "text/html");
    }
}