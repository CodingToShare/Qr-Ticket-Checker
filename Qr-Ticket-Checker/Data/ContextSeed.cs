using Microsoft.AspNetCore.Identity;
using System;

namespace Qr_Ticket_Checker.Data
{
    public static class ContextSeed
    {
        public static async Task SeedSuperAdminAsync(UserManager<IdentityUser> userManager)
        {
            //Seed Default User
            var defaultUser = new IdentityUser
            {
                UserName = "superadmin@admin.com",
                Email = "superadmin@admin.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Pa$$w0rd1");
                }

            }
        }
    }
}
