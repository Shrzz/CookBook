using System.ComponentModel.DataAnnotations;

namespace Cookbook.Models
{
    public class Like
    {
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public Like()
        {
        }

        public Like(ApplicationUser user, Recipe recipe)
        {
            this.User = user;
            this.UserId = user.Id;
            this.Recipe = recipe;
            this.RecipeId = recipe.Id;
        }
    }
}
