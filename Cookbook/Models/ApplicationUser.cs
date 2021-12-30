using Cookbook.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Cookbook.Models
{
    public class ApplicationUser : IdentityUser<int>, IEntity
    {
    }
}
