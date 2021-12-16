using Microsoft.AspNetCore.Identity;

namespace Cookbook.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Recipe> Recipes { get; set; }
    }
}
