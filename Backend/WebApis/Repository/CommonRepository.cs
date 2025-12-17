
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApis.Data;

namespace WebApis.Repository
{
    public class CommonRepository<T> : ICommonRepository<T> where T : class
    {
        //add dbcontext 
        private readonly AppDbContext _dbContext;
        private DbSet<T> _dbSet;
        public CommonRepository( AppDbContext dbContext )
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }
        //implment common funtions
        public async Task<T> AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entity)
        {
            _dbSet.AddRange(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(T entity)
        {
           _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<T>> GetAllAsync()
        {
           return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByFilterAsync(Expression<Func<T , bool>> filter )
        {
            var entity = await _dbSet.FirstOrDefaultAsync(filter);
            return entity;
        }

        public async Task<TResult?> GetByFilterAsync<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> selector)
        {
            return await _dbSet
                .Where(filter)
                .Select(selector)
                .FirstOrDefaultAsync();
        }

        public async Task<List<TResult>> GetAllByFilterAsync<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> selector)
        {
            return await _dbSet
                .Where(filter)
                .Select(selector)
                .ToListAsync();
        }

        public async Task<TResult?> GetWithIncludeAsync<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> selector,
            params string[] includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query
                .Where(filter)
                .Select(selector)
                .FirstOrDefaultAsync();
        }

        public async Task<T?> GetByorderAsync(
           Expression<Func<T, bool>> filter,
           Expression<Func<T, object>>? orderBy = null,
           bool descending = false)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = descending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<TResult?> GetByorderWithSelectorAsync<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = false)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = descending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);

            return await query.Select(selector).FirstOrDefaultAsync();
        }

        public async Task<List<TResult>?> GetByOrderWithSelectorAsync<TResult>(
            Expression<Func<T, bool>> filter,
            Expression<Func<T, TResult>> selector,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return await query.Select(selector).ToListAsync();
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var updatedEntity = _dbSet.Update(entity).Entity;
            await _dbContext.SaveChangesAsync();
            return updatedEntity;
        }

        public async Task<bool> ExistsAsync(
            Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }

    }
}
