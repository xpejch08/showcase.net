using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


using showcase;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("admin")]
public class AdminController : Controller
{
    [HttpGet("dashboard")]
    public IActionResult AdminView()
    {
        try
        {
            return Ok(new { redirectUrl = "/views/admin/adminview.html" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Internal server error.");
        }
    }
}