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

        public AuthController(AppDbContext context)
        {
            _context = context;
        }


        private bool CheckIfInputsMissing(User userBeingChecked)
        {
            if (string.IsNullOrEmpty(userBeingChecked.Name) || string.IsNullOrEmpty(userBeingChecked.Password))
            {
                return false;
            }

            return true;
        }

        private bool CheckIfUserFound(User? user)
        {
            if (user == null)
            {
                return false;
            }
            return true;
        }
        
        private async Task<bool> CheckIfUserExistsInDatabaseAsync (User userBeingChecked)
        {
            User? user =  await _context.Users.FirstOrDefaultAsync(u=>u.Name == userBeingChecked.Name 
                                                               && u.Password == userBeingChecked.Password);
            if (!CheckIfUserFound(user))
            {
                return false;
            }
            _loggedUser = user;
            return true;
        }
        
        private List<Claim> AddClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, MapRoleToUser(user))
            };
            
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }
            return claims;
        }

        private ClaimsPrincipal CreateClaimsPrincipal(User user)
        {
            var claimsIdentity = new ClaimsIdentity(AddClaims(user), 
                CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }

        private async Task SignInUserAsync(ClaimsPrincipal claimsPrincipal)
        {
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal
            );
        }
        
        private string MapRoleToUser(User userLoggingIn)
        {
            if (userLoggingIn.Role == admin)
            {
                return("Admin");
            }
            else if (userLoggingIn.Role == 2)
            {
                return ("Garant");
            }
            else if (userLoggingIn.Role == 3)
            {
                return ("Teacher");
            }
            else if (userLoggingIn.Role == 5)
            {
                return ("Student");   
            }
            return ("Guest");
            
        }

        private bool CheckIfUserIsAdmin()
        {
            if (_loggedUser.Role == admin)
            {
                return true;
            }

            return false;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User userLoggingIn)
        {
            if (!CheckIfInputsMissing(userLoggingIn))
            {
                return BadRequest("Name and Password are required");
            }
            
            if(!await CheckIfUserExistsInDatabaseAsync(userLoggingIn))
            {
                return BadRequest("User not found");
            }
            
            var claimsPrincipal = CreateClaimsPrincipal(_loggedUser);
            
            
            await SignInUserAsync(claimsPrincipal);


            if (CheckIfUserIsAdmin())
            {
                return Redirect("/admin/dashboard");
            }
            return BadRequest(new { message = "Unauthorized." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/login");
            }
            return Redirect("/admin/dashboard");
        }
    }
}