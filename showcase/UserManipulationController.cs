using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace showcase;

//todo set up rights
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

    
    
    [HttpPost("GetUser")]
    public async Task<IActionResult> GetUser([FromBody] User user)
    {
        var foundUser = await _helperMethods.CheckIfUserExistsInDatabaseAsync(user);

        if (foundUser == null)
        {
            return NotFound();  // Return 404 if no matching user
        }

        return Ok(foundUser);  // Return 200 with found user
    }


    [HttpPost("AddUser")]
    public async Task<IActionResult> AddUser([FromBody] User user)
    {
        await _helperMethods.AddUserToDatabaseAsync(user);
        return Ok();
    }
    [HttpPost("DeleteUser")]
    public async Task<IActionResult> DeleteUser([FromBody] User user)
    {
        await _helperMethods.DeleteUserFromDatabaseAsync(user);
        return Ok();
    }
    
    
}