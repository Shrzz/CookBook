using Microsoft.EntityFrameworkCore;

namespace Cookbook.Data
{
    public static class MigrationManager
    {
        public static IHost Migrate(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    try
                    {
                        appContext.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return host;
        }
    }
}
