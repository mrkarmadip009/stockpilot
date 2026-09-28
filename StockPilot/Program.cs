using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using StockPilot.Data;
using StockPilot.Repositories;

var builder = WebApplication.CreateBuilder(args);


// =============================
// MYSQL DATABASE
// =============================

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection"
    );

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );
});


// =============================
// REPOSITORIES
// =============================

builder.Services.AddScoped<
    IProductRepository,
    ProductRepository
>();

builder.Services.AddScoped<
    ICategoryRepository,
    CategoryRepository
>();


// =============================
// AUTHENTICATION
// =============================

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme
)
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";

    options.AccessDeniedPath =
        "/Account/AccessDenied";
});


// =============================
// MVC
// =============================

builder.Services.AddControllersWithViews();


var app = builder.Build();


// =============================
// MIDDLEWARE
// =============================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();


// Authentication MUST come before Authorization
app.UseAuthentication();

app.UseAuthorization();


// =============================
// DEFAULT ROUTE
// =============================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);


app.Run();