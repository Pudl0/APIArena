using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace APIArena.Server
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContextWithPooledFactory<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> builder)
            where TContext : DbContext
            => services.AddPooledDbContextFactory<TContext>(builder)
                       .AddDbContext<TContext>(builder, ServiceLifetime.Transient);
    }
}
