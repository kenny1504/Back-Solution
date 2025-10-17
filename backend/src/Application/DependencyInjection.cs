using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IConfigurationProvider>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

            var expr = new MapperConfigurationExpression();
            expr.AddMaps(typeof(DependencyInjection).Assembly);

            return new MapperConfiguration(expr, loggerFactory);
        });

        services.AddSingleton<IMapper>(sp =>
            new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<Services.ClienteService>();
        services.AddScoped<Services.CuentaService>();
        services.AddScoped<Services.MovimientoService>();

        return services;
    }
}
