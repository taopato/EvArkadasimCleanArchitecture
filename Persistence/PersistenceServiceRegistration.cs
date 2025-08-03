using Application.Services.Repositories;  // IUserRepository
using Core.Security.JWT;                  // ITokenHelper
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;
using Persistence.Security.JWT;            // EfUserRepository

namespace Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(opt =>
               opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<ITokenHelper, JwtHelper>();
            services.AddScoped<IUserRepository, EfUserRepository>();

            return services;
        }
    }

}
