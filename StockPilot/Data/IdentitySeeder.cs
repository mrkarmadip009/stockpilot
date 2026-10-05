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
                    new IdentityRole("Admin"));
            }


            // ------------------------------------
            // Create User Role
            // ------------------------------------

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(
                    new IdentityRole("User"));
            }


            // ------------------------------------
            // Admin Details
            // ------------------------------------

            string adminEmail = "admin@gmail.com";
            string adminPassword = "admin@123";


            // ------------------------------------
            // Find Admin
            // ------------------------------------

            var admin =
                await userManager.FindByEmailAsync(adminEmail);


            // ------------------------------------
            // Create Admin if not exists
            // ------------------------------------

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
                        adminPassword);


                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        admin,
                        "Admin");
                }
            }
            else
            {
                // ------------------------------------
                // Admin already exists
                // Set new password
                // ------------------------------------

                var passwordRemoved =
                    await userManager.RemovePasswordAsync(admin);

                if (passwordRemoved.Succeeded)
                {
                    await userManager.AddPasswordAsync(
                        admin,
                        adminPassword);
                }


                // ------------------------------------
                // Make sure Admin has Admin role
                // ------------------------------------

                if (!await userManager.IsInRoleAsync(
                    admin,
                    "Admin"))
                {
                    await userManager.AddToRoleAsync(
                        admin,
                        "Admin");
                }
            }
        }
    }
}

