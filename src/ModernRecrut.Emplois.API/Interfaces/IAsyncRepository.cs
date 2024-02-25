using ModernRecrut.Emplois.API.Entites;
using System.Linq.Expressions;

namespace ModernRecrut.Emplois.API.Interfaces
{
	public interface IAsyncRepository<T> where T : BaseEntity
	{
		Task<T?> GetByIdAsync(int id);
		Task<IEnumerable<T>> ListAsync();
		Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate);
		Task<T> AddAsync(T entity);
		Task DeleteAsync(T entity);
		Task EditAsync(T entity);
	}
}
