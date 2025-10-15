
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

        public async Task<T> GetByFilterAsync(Expression<Func<T , bool>> filter )
        {
            var entity = await _dbSet.FirstOrDefaultAsync(filter);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var updatedEntity = _dbSet.Update(entity).Entity;
            await _dbContext.SaveChangesAsync();
            return updatedEntity;
        }

    }
}
