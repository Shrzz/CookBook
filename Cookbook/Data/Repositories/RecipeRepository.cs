using Cookbook.Models;

namespace Cookbook.Data.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext _context;

        public RecipeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateRecipe(Recipe recipe)
        {
            if (recipe == null) 
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            _context.Add(recipe);
        }

        public Recipe GetRecipe(int id)
        {
            return _context.Recipes.FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return _context.Recipes;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}
