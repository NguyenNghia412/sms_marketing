using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using thongbao.be.domain.Auth;
using thongbao.be.domain.Config;

namespace thongbao.be.infrastructure.data.Seeder
{
    public static class SeedToiDaCreditGiaHan
    {
        public static async Task SeedAsync(SmDbContext smDbContext, UserManager<AppUser> userManager)
        {
            var exists = await smDbContext.ToiDaHanMucCreditsGiaHan
                .AnyAsync(x => !x.Deleted);

            if (!exists)
            {
                var superAdminUsers = await userManager.GetUsersInRoleAsync("SuperAdmin");
                var superAdmin = superAdminUsers.FirstOrDefault();

                var toiDaHanMucCreditsGiaHan = new ToiDaHanMucCreditsGiaHan
                {
                    ToiDaHanMucCreditGiaHan = "100000000",
                    DonVi = "VND",
                    CreatedBy = superAdmin?.Id ?? "Seeder",
                    CreatedDate = DateTime.UtcNow,
                };

                await smDbContext.ToiDaHanMucCreditsGiaHan.AddAsync(toiDaHanMucCreditsGiaHan);
                await smDbContext.SaveChangesAsync();
            }
        }
    }
}