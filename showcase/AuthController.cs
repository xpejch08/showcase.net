using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

using showcase;

namespace showcase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const int admin = 1;
        private readonly AppDbContext _context;
        private User _loggedUser;
        private readonly DbHelperMethods _helperMethods;

        public AuthController(AppDbContext context, DbHelperMethods helperMethods)
        {
            _context = context;
            _helperMethods = helperMethods;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User userLoggingIn)
        {
            if (!_helperMethods.CheckIfInputsMissing(userLoggingIn))
            {
                return BadRequest("Name and Password are required");
            }
            
            _loggedUser = await _helperMethods.CheckIfUserExistsInDatabaseAsync(userLoggingIn);
            if(!_helperMethods.CheckIfUserFound(_loggedUser))
            {
                return BadRequest("User not found");
            }
            
            var claimsPrincipal = _helperMethods.CreateClaimsPrincipal(_loggedUser);
            
            
            await _helperMethods.SignInUserAsync(claimsPrincipal);


            if (_helperMethods.CheckIfUserIsAdmin(_loggedUser))
            {
                return Redirect("/admin/dashboard");
            }
            return BadRequest(new { message = "Unauthorized." });
        }
        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _helperMethods.LogoutUserAsync();
            if (!_helperMethods.CheckIfLoggedUserIsAuthenticated())
            {
                return Redirect("/login");
            }
            return Redirect("/admin/dashboard");
        }
    }
}