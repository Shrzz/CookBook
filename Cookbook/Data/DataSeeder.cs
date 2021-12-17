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
                if (context is null)
                {
                    throw new ArgumentNullException(nameof(context));
                }
                SeedData(context);
            }
        }

        private static void SeedData(ApplicationDbContext context)
        {
            //SeedUsers(context);
            //SeedRecipes(context);               
        }

        private static void SeedUsers(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    Id = 1,
                    UserName = "admin@cookbook.com",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAEAACcQAAAAEHSfNTl/HOK/2qWeN/4E/wejl9K/6ssIS9YpBohIYYhLo1sBvsrFq388+a8000cGow==",
                };

                context.Add(user);
                context.SaveChanges();
            }
        }

        private static void SeedRecipes(ApplicationDbContext context)
        {
            var a = context.Users.FirstOrDefault();

            if (!context.Recipes.Any())
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                context.Recipes.AddRange(
                    new Recipe() { Title = "Eggs", Description = "Tasty roasted eggs", Author = a },
                    new Recipe() { Title = "Pizza", Description = "Pineapple pizza with extra cheese", Author = a },
                    new Recipe() { Title = "Pasta", Description = "Extra curly pasta with pepper", Author = a }
                );
#pragma warning restore CS8601 // Possible null reference assignment.

                context.SaveChanges();
            }
        }
    }
}
