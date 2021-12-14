using Cookbook.Models;

namespace Cookbook.Data.Repositories
{
    public interface IRecipeRepository
    {
        bool SaveChanges();
        IEnumerable<Recipe> GetRecipes();
        Recipe GetRecipe(int id);
        void CreateRecipe(Recipe recipe);
    }
}
