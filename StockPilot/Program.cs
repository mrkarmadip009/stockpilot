using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockPilot.Data;
using StockPilot.Models;
using StockPilot.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------
// Database
// ------------------------------------

var connectionString =
builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseMySql(
connectionString,
ServerVersion.AutoDetect(connectionString)
));

// ------------------------------------
// Repository Dependency Injection
// ------------------------------------

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// ------------------------------------
// ASP.NET Core Identity
// ------------------------------------

builder.Services
.AddIdentity<User, IdentityRole>(options =>
{
options.Password.RequireDigit = false;
options.Password.RequireLowercase = false;
options.Password.RequireUppercase = false;
options.Password.RequireNonAlphanumeric = false;
options.Password.RequiredLength = 4;


    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// ------------------------------------
// Session
// ------------------------------------

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

// ------------------------------------
// MVC
// ------------------------------------

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ------------------------------------
// Middleware
// ------------------------------------

if (!app.Environment.IsDevelopment())
{
app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

// ------------------------------------
// Default Route
// ------------------------------------

app.MapControllerRoute(
name: "default",
pattern: "{controller=Account}/{action=Login}/{id?}"
);

using (var scope = app.Services.CreateScope())
{
var services = scope.ServiceProvider;


await IdentitySeeder.SeedAsync(services);


}


app.Run();
