using Domain.Abstractions;
using Infrastructure.Persistence;
using Infrastructure.UoW;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? conn)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(conn));

        services.AddScoped<DbContext, AppDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}