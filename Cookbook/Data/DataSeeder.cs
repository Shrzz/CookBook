using Cookbook.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace Cookbook.Data
{
    public class DataSeeder
    { 
        public static void InitializeDataSeeding(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope);
            }
        }

        private static void SeedData(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            if (userManager is null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            SeedUsers(context, userManager);
            SeedRecipes(context);
        }

        private static void SeedUsers(ApplicationDbContext context, UserManager<ApplicationUser> um)
        {
            if (!context.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    UserName = "admin@cookbook.com",
                    EmailConfirmed = true,
                };
                um.AddPasswordAsync(user, "aDMIN11_");

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
                    "wait",
                    "???",
                    "profit"
                };

                var ingridients = new List<string>
                {
                    "salt - 2 spoons",
                    "melted pepper - 0.5 of spoon",
                    "potato - 3 pieces"
                };

                context.Recipes.AddRange(
                    new Recipe() { Title = "Eggs", Description = "Tasty roasted eggs", Author = a, Steps = steps, Ingredients = ingridients },
                    new Recipe() { Title = "Pizza", Description = "Pineapple pizza with extra cheese", Author = a, Steps = steps, Ingredients = ingridients },
                    new Recipe() { Title = "Pasta", Description = "Extra curly pasta with pepper", Author = a, Steps = steps, Ingredients = ingridients }
                );
#pragma warning restore CS8601 // Possible null reference assignment.

                context.SaveChanges();
            }
        }
    }
}
