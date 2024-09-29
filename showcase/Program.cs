using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using showcase;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5050");

// Add services to the container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=showcase.db"));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/login.html";
    options.AccessDeniedPath = "/login.html";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Serve static files
var defaultFileOptions = new DefaultFilesOptions();
defaultFileOptions.DefaultFileNames.Clear();
defaultFileOptions.DefaultFileNames.Add("login.html"); // Set test.html as the default file

app.UseDefaultFiles(defaultFileOptions);  // Use default files (like test.html)

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/views/admin"))
    {
        if (context.User.IsInRole("Admin"))
        {
            await next();
        }
        else
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access Denied");
        }
    }
    else
    {
        await next();
    }
});


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseStaticFiles(); // Keep serving static files, but restrict access based on role
// Serve static files from wwwroot

// Initialize database if needed
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!File.Exists("showcase.db"))
    {
        var initializer = new DatabaseInitialiser(dbContext);
        initializer.Initialise(); // Execute your SQL script
    }
}

app.UseCors("AllowAll");

app.Run(); // Run the app