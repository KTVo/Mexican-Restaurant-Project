// We're using the repostory design, this is an interface for our CRUD operations
// Interface is a "promise" that a class should contain i.g. method
namespace TequilasRestaurant.Models
{
    public interface IRepository<T> where T: class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id, QueryOptions<T> options);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);

    }
}
