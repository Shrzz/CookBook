using Cookbook.Models;

namespace Cookbook.Data.Repositories
{
    public class UserRepository : GenericRepository<ApplicationUser, ApplicationDbContext>
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
