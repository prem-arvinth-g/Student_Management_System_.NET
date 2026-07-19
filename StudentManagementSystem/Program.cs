using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// ---- Add Services ----

// Tell the app to use MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// Tell the app to use MySQL with our ApplicationDbContext
// The connection string comes from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Enable Sessions — this is how we remember who is logged in
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Stay logged in for 60 minutes
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Enable HttpContext access in controllers (needed for sessions)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ---- Configure Middleware Pipeline ----

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();    // Serves CSS, JS, images from wwwroot/
app.UseRouting();        // Figures out which controller handles which URL
app.UseSession();        // Enables session (must come before UseAuthorization)
app.UseAuthorization();

// Default route: when someone visits "/" → go to Account/Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
