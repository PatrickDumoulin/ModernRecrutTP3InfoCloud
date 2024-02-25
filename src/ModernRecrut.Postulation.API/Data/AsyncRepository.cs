using Microsoft.EntityFrameworkCore;
using ModernRecrut.Postulation.API.Entites;
using ModernRecrut.Postulation.API.Interfaces;
using System.Linq.Expressions;

namespace ModernRecrut.Postulation.API.Data
{
    public class AsyncRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly PostulationsContext _dbContext;

        public AsyncRepository(PostulationsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
			return await _dbContext.Set<T>().AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<T>> ListAsync()
        {
			return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
			await _dbContext.Set<T>().AddAsync(entity);
			await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
			_dbContext.Set<T>().Remove(entity);
			await _dbContext.SaveChangesAsync();
        }

        public async Task EditAsync(T entity)
        {
			_dbContext.Entry(entity).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();
        }
    }
}
