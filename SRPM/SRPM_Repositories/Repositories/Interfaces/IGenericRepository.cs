using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace SRPM_Repositories.Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
    Task<int> CountAsync(Expression<Func<T, bool>> expression);

    Task<List<T>?> GetListAsync(Expression<Func<T, bool>> expression, bool hasTrackings = true);

    Task<List<TResult>?> GetListAdvanceAsync<TResult>(
        Expression<Func<T, bool>> whereLinQ,
        Expression<Func<T, TResult>> selectLinQ,
        bool hasTrackings = true);

    Task<T?> GetOneAsync(Expression<Func<T, bool>> expression, bool hasTrackings = true);
    Task<T?> GetByIdAsync<Tkey>(Tkey id);

    System.Threading.Tasks.Task AddAsync(T TEntity);
    Task AddRangeAsync(IEnumerable<T> Tentities);

    Task UpdateAsync(T TEntity);

    Task DeleteAsync(T TEntity);
    Task DeleteRangeAsync(IEnumerable<T> TEntities);

    Task<bool> SaveChangeAsync();
}
