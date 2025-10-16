using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UoW;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _ctx;
    private readonly Dictionary<Type, object> _repos = new();

    public UnitOfWork(DbContext ctx) => _ctx = ctx;

    public IRepository<T> Repository<T>() where T : class
    {
        if (_repos.TryGetValue(typeof(T), out var repo))
            return (IRepository<T>)repo;

        var newRepo = new Repositories.Repository<T>(_ctx);
        _repos[typeof(T)] = newRepo;
        return newRepo;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct) => _ctx.SaveChangesAsync(ct);
}