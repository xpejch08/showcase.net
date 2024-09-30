using Microsoft.AspNetCore.Mvc;

namespace showcase;


[ApiController]
[Route("api/[controller]")]
public class UserManipulationController
{
    private AppDbContext _context;

    public UserManipulationController(AppDbContext context)
    {
        _context = context;
    }

    // private async Task<User?> GetUserFromDatabaseAsync()
    // {
    //     
    // }
    //
    // [HttpGet("GetUser")]
    // public async Task<ActionResult> GetUser()
    // {
    //     var user = new User();
    //     
    //     
    // }
}