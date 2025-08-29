using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thongbao.be.domain.Auth;

namespace thongbao.be.infrastructure.data.Seeder
{
    public static class SeedUser
    {
        public static async Task SeedAsync(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // 1. Ensure Roles
            var adminRole = "SuperAdmin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // 2. Ensure Admin User
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Super Administrator",
                };

                var result = await userManager.CreateAsync(adminUser, "123456Aa@"); // strong password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
                else
                {
                    throw new Exception("Failed to create admin: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
