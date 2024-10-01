using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace showcase;


[ApiController]
[Route("api/[controller]")]
public class UserManipulationController : ControllerBase
{
    private AppDbContext _context;
    private DbHelperMethods _helperMethods;

    public UserManipulationController(AppDbContext context, DbHelperMethods dbHelperMethods)
    {
        _context = context;
        _helperMethods = dbHelperMethods;
    }

    
    
    [HttpGet("GetUser")]
    public async Task<IActionResult> GetUser([FromBody] User user)
    {
        var foundUser = await _helperMethods.CheckIfUserExistsInDatabaseAsync(user);

        return Ok(foundUser);

    }

    [HttpPost("AddUser")]
    public async Task<IActionResult> AddUser([FromBody] User user)
    {
        await _helperMethods.AddUserToDatabaseAsync(user);
        return Ok();
    }
    
    
}