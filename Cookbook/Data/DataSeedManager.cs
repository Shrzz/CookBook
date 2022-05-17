using Cookbook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Data
{
    public static class DataSeedManager
    {
        public async static Task<IHost> Seed(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    using (var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
                    {
                        using (var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>())
                        {
                            try
                            {
                                if (!roleManager.Roles.Any())
                                {
                                    ApplicationRole role = Activator.CreateInstance<ApplicationRole>();
                                    role.Name = "admin";
                                    await roleManager.CreateAsync(role);
                                }

                                var userStore = scope.ServiceProvider.GetService<IUserStore<ApplicationUser>>();
                                var emailStore = (IUserEmailStore<ApplicationUser>)userStore;

                                var email = "admin@cookbook.com";
                                var password = "Admin_123";

                                if (!context.Users.Any())
                                {
                                    var user = Activator.CreateInstance<ApplicationUser>();
                                    await userStore.SetUserNameAsync(user, email, CancellationToken.None);
                                    await emailStore.SetEmailAsync(user, email, CancellationToken.None);
                                    user.EmailConfirmed = true;
                                    var result = await userManager.CreateAsync(user, password);
                                    await userManager.AddToRoleAsync(user, "admin");
                                }

                                if (!context.Recipes.Any()){
                                    var user = userManager.Users.First();
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
                                            Author = user,
                                            Steps = steps,
                                            Ingredients = ingridients,
                                            CreationTime = DateTime.UtcNow
                                        },
                                        new Recipe()
                                        {
                                            Title = "Pizza",
                                            Description = "Pineapple pizza with extra cheese",
                                            Author = user,
                                            Steps = steps,
                                            Ingredients = ingridients,
                                            CreationTime = DateTime.UtcNow
                                        },
                                        new Recipe()
                                        {
                                            Title = "Pasta",
                                            Description = "Extra curly pasta with pepper",
                                            Author = user,
                                            Steps = steps,
                                            Ingredients = ingridients,
                                            CreationTime = DateTime.UtcNow
                                        }
                                    );

                                    context.SaveChanges();
                                }
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                }
            }

            return host;
        }
    }
}
