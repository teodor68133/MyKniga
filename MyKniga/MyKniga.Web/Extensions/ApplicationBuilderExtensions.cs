namespace MyKniga.Web.Extensions
{
    using System.Threading.Tasks;
    using Common;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Data;
    using Microsoft.EntityFrameworkCore.Internal;
    using MyKniga.Models;

    public static class ApplicationBuilderExtensions
    {
        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            InitializeDatabaseAsync(app).GetAwaiter().GetResult();
        }

        // Migrate database and create administrator role
        private static async Task InitializeDatabaseAsync(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<MyKnigaDbContext>();

                await dbContext.Database.MigrateAsync();

                // Create administrator and publisher roles

                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(GlobalConstants.AdministratorRoleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(GlobalConstants.AdministratorRoleName));
                }

                if (!await roleManager.RoleExistsAsync(GlobalConstants.PublisherRoleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(GlobalConstants.PublisherRoleName));
                }

                // Create initial administrator

                var userManager = serviceScope.ServiceProvider.GetService<UserManager<KnigaUser>>();

                if (!dbContext.Users.Any())
                {
                    var admin = new KnigaUser
                    {
                        Email = "admin@admin.com",
                        UserName = "admin@admin.com"
                    };

                    await userManager.CreateAsync(admin, "admin123");
                    await userManager.AddToRoleAsync(admin, GlobalConstants.AdministratorRoleName);
                }
            }
        }
    }
}