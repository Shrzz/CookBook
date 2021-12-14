using Cookbook.Models;

namespace Cookbook.Data
{
    public class DataSeeder
    {
        public static void InitializeDataSeeding(IApplicationBuilder app)
        {
            using (var serviceScoped = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScoped.ServiceProvider.GetService<ApplicationDbContext>());
            }
        }


        private static void SeedData(ApplicationDbContext context)
        {
            if (!context.Recipes.Any())
            {
                context.Recipes.AddRange(
                    new Recipe() { Title = "Eggs", Description = "Tasty roasted eggs" },
                    new Recipe() { Title = "Pizza", Description = "Pineapple pizza with extra cheese" } ,
                    new Recipe() { Title = "Pasta", Description = "Extra curly pasta with pepper" }
                );
                
                context.SaveChanges();
            }
        }
    }
}
