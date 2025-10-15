using System.Linq.Expressions;

namespace WebApis.Repository
{
    public interface ICommonRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByFilterAsync(Expression<Func<T,bool>> filter); // this will be used to get by id or any other filter
        Task<T> AddAsync(T entity); 
        Task<T> UpdateAsync(T entity); 
        Task<bool> DeleteAsync(T entity);
    }
}
