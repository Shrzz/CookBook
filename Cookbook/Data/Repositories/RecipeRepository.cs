using Cookbook.Models;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Data.Repositories
{
    public class RecipeRepository : GenericRepository<Recipe, ApplicationDbContext>
    {
        private readonly ApplicationDbContext _context;

        public RecipeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public override void Update(Recipe obj)
        {
            _context.Recipes.Update(obj);
            _context.Entry<Recipe>(obj).Reference(p => p.Author).IsModified = false;
            _context.Entry<Recipe>(obj).Property(p => p.ImagesDirectory).IsModified = false;
            _context.Entry<Recipe>(obj).Property(p => p.CreationTime).IsModified = false;
            SaveChanges();
        }
    }
}
