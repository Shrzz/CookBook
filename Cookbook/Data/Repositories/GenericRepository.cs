using Microsoft.EntityFrameworkCore;

namespace Cookbook.Data.Repositories
{
    public abstract class GenericRepository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : class, IEntity
        where TContext : DbContext
    {
        private readonly TContext _context;
        public GenericRepository(TContext context)
        {
            _context = context;
        }

        public virtual void Create(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            SaveChanges();
        }

        public virtual IQueryable<TEntity> GetAll(string includedProperties = "")
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            foreach (var includeProperty in includedProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public virtual TEntity? GetById(int id, string includedProperties = "")
        {
            var query = GetAll(includedProperties);

            return query.First(e => e.Id == id);
        }

        public virtual void Update(TEntity obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
            SaveChanges();
        }

        public virtual bool Delete(int id)
        {
            var obj = _context.Set<TEntity>().Find(id);
            if (obj == null)
            {
                return false;
            }

            _context.Set<TEntity>().Remove(obj);
            SaveChanges();
            return true;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}
