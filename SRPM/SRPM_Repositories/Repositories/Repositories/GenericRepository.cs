using Microsoft.EntityFrameworkCore;
using SRPM_Repositories.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SRPM_Repositories.Repositories.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly SRPMDbContext _context;
    public GenericRepository(SRPMDbContext context)
    {
        _context = context;
    }

    //=============================================
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().AnyAsync(expression);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().CountAsync(expression);
    }

    public async Task<List<T>?> GetListAsync(Expression<Func<T, bool>> expression, bool hasTrackings = true)
    {
        return hasTrackings ? await _context.Set<T>().Where(expression).ToListAsync()
                            : await _context.Set<T>().Where(expression).AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetOneAsync(Expression<Func<T, bool>> expression, bool hasTrackings = true)
    {
        return hasTrackings ? await _context.Set<T>().FirstOrDefaultAsync(expression)
                            : await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(expression);
    }

    public async Task<T?> GetByIdAsync<Tkey>(Tkey id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
    public Task AddAsync(T TEntity)
    {
        _context.Add(TEntity);
        return Task.CompletedTask;
    }

    public async Task AddRangeAsync(IEnumerable<T> Tentities)
    {
        await _context.Set<T>().AddRangeAsync(Tentities);
    }

    public Task UpdateAsync(T TEntity)
    {
        _context.Set<T>().Update(TEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T TEntity)
    {
        _context.Remove(TEntity);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(IEnumerable<T> TEntities)
    {
        _context.Set<T>().RemoveRange(TEntities);
        return Task.CompletedTask;
    }

    public async Task<bool> SaveChangeAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
