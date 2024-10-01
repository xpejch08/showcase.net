using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace showcase;

public class DbHelperMethods
{
    private AppDbContext _context;
    private User _loggedUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const int admin = 1;

    public DbHelperMethods(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public bool CheckIfInputsMissing(User userBeingChecked)
    {
        if (string.IsNullOrEmpty(userBeingChecked.Name) || string.IsNullOrEmpty(userBeingChecked.Password))
        {
            return false;
        }

        return true;
    }
    
    public bool CheckIfUserFound(User? user)
    {
        if (user == null)
        {
            return false;
        }
        return true;
    }
    
    public async Task<User?> CheckIfUserExistsInDatabaseAsync (User userBeingChecked)
    {
        User? user =  await _context.Users.FirstOrDefaultAsync(u=>u.Name == userBeingChecked.Name 
                                                                  && u.Password == userBeingChecked.Password);
        return user; 
    }
    
    public List<Claim> AddClaims(User user)
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
    
    public ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        var claimsIdentity = new ClaimsIdentity(AddClaims(user), 
            CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(claimsIdentity);
    }
    
    public async Task SignInUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        await _httpContextAccessor.HttpContext.SignInAsync(
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

    public bool CheckIfUserIsAdmin(User user)
    {
        if (user.Role == admin)
        {
            return true;
        }

        return false;
    }
    
    public bool CheckIfLoggedUserIsAuthenticated()
    {
        if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            return true;
        }
        return false;
    }

    public async Task LogoutUserAsync()
    {
        await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task AddUserToDatabaseAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}