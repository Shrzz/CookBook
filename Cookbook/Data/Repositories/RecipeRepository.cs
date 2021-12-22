using Cookbook.Models;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Data.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext _context;

        public RecipeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(Recipe recipe)
        {
            recipe.CreationTime = DateTime.Now;
            _context.Add(recipe);
            SaveChanges();
        }

        public Recipe GetById(int id)
        {
            return _context.Recipes.Include(r => r.Author).FirstOrDefault(r => r.Id.Equals(id));
        }

        public IEnumerable<Recipe> GetAll()
        {
            return _context.Recipes;
        }

        public bool Delete(int id)
        {
            var targetRecipe = _context.Recipes.Find(id);
            if (targetRecipe is null)
            {
                return false;
            }

            _context.Remove(targetRecipe);
            SaveChanges();
            return true;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        public void Update(Recipe obj)
        {
            _context.Recipes.Update(obj);
            SaveChanges();
        }

        public IEnumerable<Recipe> GetAllCreatedByUser(int id)
        {
            var a = _context.Recipes.Where(r => r.Author.Id == id);
            return a;
        }
    }
}
