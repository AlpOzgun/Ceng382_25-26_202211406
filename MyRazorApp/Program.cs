using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Identity services and configure the options
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
{
    /*options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;*/
})
.AddEntityFrameworkStores<SchoolDbContext>() // Ensure this uses the correct DbContext (SchoolDbContext)
.AddDefaultTokenProviders(); // Default token providers for password reset, etc.

// Add Razor Pages services to the container
builder.Services.AddRazorPages();

// Add session services with a 30-minute idle timeout
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Essential for GDPR compliance
});

// Add DbContext for SchoolDbContext with SQL Server connection string
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection"))); // Ensure correct connection string in appsettings.json

var app = builder.Build();

// Configure the HTTP request pipeline (middleware setup)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // In production, you might want to add security-related middleware like HSTS (HTTP Strict Transport Security)
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseStaticFiles(); // Serve static files like CSS, JS, etc.
app.UseSession(); // Enable session middleware
app.UseRouting(); // Enable routing middleware

// Add authentication and authorization middleware
app.UseAuthentication(); 
app.UseAuthorization(); 

// Map Razor Pages to handle page requests

app.MapRazorPages();

app.Run(); // Run the application
