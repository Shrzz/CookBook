using Cookbook.Models;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Data.Repositories
{
    public class LikeRepository
    {
        private readonly ApplicationDbContext _context;
        public LikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual void Create(Like entity)
        {
            _context.Set<Like>().Add(entity);
            SaveChanges();
        }

        public virtual IQueryable<Like> GetAllUserLiked(int userId, string includedProperties = "")
        {
            IQueryable<Like> query = _context.Set<Like>();
            foreach (var includeProperty in includedProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return query.Where(l => l.UserId == userId);
        }

        public virtual Like? GetById(int userId, int recipeId, string includedProperties = "")
        {
            var query = GetAllUserLiked(userId, includedProperties);

            return query.FirstOrDefault(l => l.RecipeId == recipeId);
        }

        public virtual bool IsLikedByUser(int userId, int recipeId)
        {
            var query = GetById(userId, recipeId);

            return !(query is null);
        }

        public virtual void Update(Like obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
            SaveChanges();
        }

        public virtual bool Delete(int userId, int recipeId)
        {
            var obj = _context.Set<Like>().Find(userId, recipeId);
            if (obj == null)
            {
                return false;
            }

            _context.Set<Like>().Remove(obj);
            SaveChanges();
            return true;
        }

        public virtual bool Delete(ApplicationUser user, Recipe recipe)
        {
            return Delete(user.Id, recipe.Id);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}
