namespace Cookbook.Data.Repositories
{
    public interface IRepository<T>
    {
        public IEnumerable<T> GetAll();
        public T? GetById(int id);
        public void Create(T obj);
        public void Update(T obj);
        public bool Delete(int id);
        public bool SaveChanges();
    }
}
