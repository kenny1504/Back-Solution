using System.Linq.Expressions;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
  private readonly DbSet<T> _set;

  public Repository(DbContext ctx)
  {
    _set = ctx.Set<T>();
  }

  public async Task<T?> GetByIdAsync(int id, CancellationToken ct) =>
    await _set.FindAsync([id], ct);

  public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate, CancellationToken ct) =>
    predicate is null
      ? await _set.AsNoTracking().ToListAsync(ct)
      : await _set.AsNoTracking().Where(predicate).ToListAsync(ct);

  public async Task AddAsync(T entity, CancellationToken ct) => await _set.AddAsync(entity, ct);

  public void Update(T entity) => _set.Update(entity);

  public void Remove(T entity) => _set.Remove(entity);

  public IQueryable<T> Query()
  {
    return _set.AsNoTracking();
  }
}
