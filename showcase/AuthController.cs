using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
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

            return Redirect("/views/admin/adminview.html");
        }
    }
}