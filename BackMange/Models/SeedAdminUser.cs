using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BackMange.Models
{
    public class SeedAdminUser
    {
        //初始化管理員帳戶，若沒有會自動新增，已在Program.cs設定
        public static async Task InitializeAsync(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@12345"; 

            // 確保角色存在
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // 檢查是否已有 Admin 帳號
            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                // 建立管理員帳號
                user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    Console.WriteLine("Admin 帳號已建立成功");
                }
                else
                {
                    Console.WriteLine("Admin 帳號建立失敗：" + string.Join(", ", result.Errors));
                }
            }
            else
            {
                Console.WriteLine("Admin 帳號已存在");
            }
        }
    }
}
