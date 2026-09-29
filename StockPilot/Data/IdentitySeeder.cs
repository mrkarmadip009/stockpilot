using Microsoft.AspNetCore.Identity;
using StockPilot.Models;

namespace StockPilot.Data
{
public static class IdentitySeeder
{
public static async Task SeedAsync(
IServiceProvider serviceProvider)
{
var roleManager =
serviceProvider.GetRequiredService<
RoleManager<IdentityRole>>();


        var userManager =
            serviceProvider.GetRequiredService<
                UserManager<User>>();


        // ------------------------------------
        // Create Admin Role
        // ------------------------------------

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(
                new IdentityRole("Admin")
            );
        }


        // ------------------------------------
        // Create User Role
        // ------------------------------------

        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(
                new IdentityRole("User")
            );
        }


        // ------------------------------------
        // Create Default Admin
        // ------------------------------------

        string adminEmail =
            "admin@stockpilot.com";

        string adminPassword =
            "admin123";


        var admin =
            await userManager.FindByEmailAsync(
                adminEmail
            );


        if (admin == null)
        {
            admin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Administrator",
                EmailConfirmed = true
            };


            var result =
                await userManager.CreateAsync(
                    admin,
                    adminPassword
                );


            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(
                    admin,
                    "Admin"
                );
            }
        }
    }
}


}
