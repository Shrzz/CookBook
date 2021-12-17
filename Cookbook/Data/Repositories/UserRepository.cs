using Cookbook.Models;

namespace Cookbook.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(ApplicationUser obj)
        {
            _context.Add(obj);
            SaveChanges();
        }

        public bool Delete(int id)
        {
            _context.Remove(id);
            SaveChanges();
            return true;
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            var list = _context.Users;

            return list;
        }

        public ApplicationUser? GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id.Equals(id));
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        public void Update(ApplicationUser obj)
        {
            _context.Update(obj);
            SaveChanges();
        }
    }
}
