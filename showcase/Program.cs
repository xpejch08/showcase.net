using Microsoft.EntityFrameworkCore;
using showcase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=showcase.db"));
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Configure static file serving
var defaultFileOptions = new DefaultFilesOptions();
defaultFileOptions.DefaultFileNames.Clear();
defaultFileOptions.DefaultFileNames.Add("login.html"); // Set login.html as the default file

app.UseDefaultFiles(defaultFileOptions); // Serve login.html by default
app.UseStaticFiles();                    // Serve static files from wwwroot

app.UseRouting();
app.MapControllers(); // Maps controller routes

// Call the database initializer if needed
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!File.Exists("showcase.db"))
    {
        var initializer = new DatabaseInitialiser(dbContext);
        initializer.Initialise(); // Execute your SQL script
    }
}

app.Run(); // Run the app