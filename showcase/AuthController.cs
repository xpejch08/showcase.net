using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

using showcase;

namespace showcase.Controllers
{
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
        
        [HttpGet("test-admin")]
        public IActionResult TestAdmin()
        {
            if (User.IsInRole("Admin"))
            {
                return Ok("You are an Admin!");
            }
            return Unauthorized("You are not an Admin.");
        }

    }
    

    
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

        private async Task<String> CreateHtmlContentIfHtmlFileExists()
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "AuthViews", "adminview.html");
            if (System.IO.File.Exists(filepath))
            {
                var htmlContent = await System.IO.File.ReadAllTextAsync(filepath);
                return htmlContent;
            }
            return string.Empty;
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
    }
}