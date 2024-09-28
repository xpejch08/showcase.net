using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

using showcase;

namespace showcase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User userLoggingIn)
        {
            if (string.IsNullOrEmpty(userLoggingIn.Name) || string.IsNullOrEmpty(userLoggingIn.Password))
            {
                return BadRequest("Username and password is incorrect");
            }
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == userLoggingIn.Name 
                                                               && u.Password == userLoggingIn.Password);
            
            if (user == null)
            {
                return BadRequest("User doesn't exist!");
            }
            
            var claimsIdentity = new ClaimsIdentity(AddClaims(user), 
                CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal
            );


            if (user.Role == 1)
            {
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "AuthViews", "adminview.html");
                if (System.IO.File.Exists(filepath))
                {
                    var htmlContent = await System.IO.File.ReadAllTextAsync(filepath);
                    return Ok(new { html = htmlContent });
                }
                return NotFound(new { message = "File not found." });
            }
            return BadRequest(new { message = "Unauthorized." });
        }
        
        private List<Claim> AddClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, MapRoleToUser(user))
            };
            return claims;
        }

        private string MapRoleToUser(User userLoggingIn)
        {
            if (userLoggingIn.Role == 1)
            {
                return("Admin");
            }
            return("User");
        }
    }
}