using Cookbook.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace Cookbook.Data
{
    public class DataSeeder
    { 
        public async static Task InitializeDataSeeding(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                await SeedData(serviceScope);
            }
        }

        private async static Task SeedData(IServiceScope serviceScope)
        {
            using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
            {
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                if (userManager is null)
                {
                    throw new ArgumentNullException(nameof(userManager));
                }

                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
                if (roleManager is null)
                {
                    throw new ArgumentNullException(nameof(roleManager));
                }

                await SeedUsers(context, userManager, roleManager);
                //SeedRecipes(context);
            }
        }

        private async static Task SeedUsers(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                ApplicationRole role = new ApplicationRole("admin");
                await roleManager.CreateAsync(role);
            }

            if (!context.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    UserName = "admin@cookbook.com",
                    NormalizedUserName = "ADMIN@COOKBOOK.COM",
                    Email = "admin@cookbook.com",
                    NormalizedEmail = "ADMIN@COOKBOOK.COM",
                    EmailConfirmed = true,
                        
                };

                await userManager.AddPasswordAsync(user, "aDMIN11_");
                await userManager.AddToRoleAsync(user, "admin");

                context.Add(user);
                context.SaveChanges();
            }
        }

        private static void SeedRecipes(ApplicationDbContext context)
        {

            if (!context.Recipes.Any())
            {
                var a = context.Users.FirstOrDefault();
#pragma warning disable CS8601 // Possible null reference assignment.
                var steps = new List<string>
                {
                    "boil water",
                    "add ingridients",
                    "wait for end",
                };

                var ingridients = new List<string>
                {
                    "salt - 2 spoons",
                    "melted pepper - 0.5 of spoon",
                    "potato - 3 pieces"
                };

                context.Recipes.AddRange(
                    new Recipe() 
                    { 
                        Title = "Eggs", 
                        Description = "Tasty roasted eggs", 
                        Author = a, 
                        Steps = steps, 
                        Ingredients = ingridients,
                        CreationTime = DateTime.UtcNow
                    },
                    new Recipe()
                    { 
                        Title = "Pizza", 
                        Description = "Pineapple pizza with extra cheese", 
                        Author = a, 
                        Steps = steps, 
                        Ingredients = ingridients,
                        CreationTime = DateTime.UtcNow
                    },
                    new Recipe() 
                    { 
                        Title = "Pasta", 
                        Description = "Extra curly pasta with pepper", 
                        Author = a, 
                        Steps = steps, 
                        Ingredients = ingridients,
                        CreationTime = DateTime.UtcNow
                    }
                );
#pragma warning restore CS8601 // Possible null reference assignment.

                context.SaveChanges();
            }
        }


    }
}
