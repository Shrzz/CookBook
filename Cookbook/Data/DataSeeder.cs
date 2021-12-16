using Cookbook.Models;

namespace Cookbook.Data
{
    public class DataSeeder
    {
        public static void InitializeDataSeeding(IApplicationBuilder app)
        {
            using (var serviceScoped = app.ApplicationServices.CreateScope())
            {
                var context = serviceScoped.ServiceProvider.GetService<ApplicationDbContext>();
                SeedData(context);
            }
        }

        private static void SeedData(ApplicationDbContext context)
        {
            SeedUsers(context);
            SeedRecipes(context);               
        }

        private static void SeedUsers(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    UserName = "admin@cookbook.com",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAEAACcQAAAAEIUn/Qqd+TGdUo6Uwv5Tc/aUrruRcVaOC3X9FgtGXO8vTDIvLv4AEL5I4ujc64U9Rg==",
                    SecurityStamp = "HJ7T2UZGTMIFBWPI5ZELISS7NXK54A6R",
                    ConcurrencyStamp = "48bfcd43-7d65-4ea2-b5b7-89155a00d3bd"
                };
            }
        }

        private static void SeedRecipes(ApplicationDbContext context)
        {
            var a = context.Users.FirstOrDefault();

            if (!context.Recipes.Any())
            {
                context.Recipes.AddRange(
                    new Recipe() { Title = "Eggs", Description = "Tasty roasted eggs", Author = a},
                    new Recipe() { Title = "Pizza", Description = "Pineapple pizza with extra cheese", Author = a},
                    new Recipe() { Title = "Pasta", Description = "Extra curly pasta with pepper", Author = a }
                );

                context.SaveChanges();
            }
        }
    }
}
