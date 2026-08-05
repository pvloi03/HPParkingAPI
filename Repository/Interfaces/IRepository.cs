namespace LearnApi.Repository.Interfaces;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(string id);
    Task InsertAsync(T entity);
    Task<bool> DeleteAsync(string id);
}