using Cookbook.Models;

namespace Cookbook.Data.Repositories
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        public IEnumerable<Recipe> GetAllCreatedByUser(int id);
    }
}
