namespace POSsystem.Contracts.Data.Repositories
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T Get(object id);
        T Add(T entity);
        void Update(T entity);
        void Delete(object id);
        int Count();
    }
}