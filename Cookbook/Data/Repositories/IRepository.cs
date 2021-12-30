namespace Cookbook.Data.Repositories
{
    public interface IRepository<T>
    {
        public IEnumerable<T> GetAll(string includedProperties = "");
        public T? GetById(int id, string includedProperties = "");
        public void Create(T obj);
        public void Update(T obj);
        public bool Delete(int id);
        public bool SaveChanges();
    }
}
