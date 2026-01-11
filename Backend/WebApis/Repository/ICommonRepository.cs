using System.Linq.Expressions;

namespace WebApis.Repository
{
    public interface ICommonRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByFilterAsync(Expression<Func<T,bool>> filter); // this will be used to get by id or any other filter
        Task<TResult?> GetByFilterAsync<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> selector);

        public Task<List<TResult>> GetAllByFilterAsync<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> selector);
        
        public Task<TResult?> GetWithIncludeAsync<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> selector,
            params string[] includes);

        public Task<List<TResult>?> GetWithAllIncludeAsync<TResult>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, TResult>> selector,
        params string[] includes);

        //overload order by
        public Task<T?> GetByorderAsync(
           Expression<Func<T, bool>> filter,
           Expression<Func<T, object>>? orderBy = null,
           bool descending = false);
        public Task<TResult?> GetByorderWithSelectorAsync<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = false);

        public Task<List<TResult>?> GetByOrderWithSelectorAsync<TResult>(
          Expression<Func<T, bool>> filter,
          Expression<Func<T, TResult>> selector,
          Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        Task<T> AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entity);

        Task<T> UpdateAsync(T entity);
        
        Task<bool> DeleteAsync(T entity);

        public Task<bool> ExistsAsync(
            Expression<Func<T, bool>> filter);

        public Task RemoveRangeAsync(IEnumerable<T> entities);

        public Task SaveChangesAsync();
    }
}
