using fenixjobs_api.Application.Interfaces;
using fenixjobs_api.Application.Interfaces.Auth;
using fenixjobs_api.Application.Interfaces.Customers;
using fenixjobs_api.Application.Interfaces.Vales;
using fenixjobs_api.Application.Services.Auth;
using fenixjobs_api.Application.Services.Customers;
using fenixjobs_api.Application.Services.Vales;
using fenixjobs_api.Infrastructure.Persistence.MongoDB;
using fenixjobs_api.Infrastructure.Persistence.MySQL;
using fenixjobs_api.Infrastructure.Repositories.Customers;
using fenixjobs_api.Infrastructure.Repositories.Vales;
using fenixjobs_api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace fenixjobs_api.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlConnection = configuration.GetConnectionString("MySql");

            services.AddDbContext<FenixDbContext>(options =>
                options.UseMySql(sqlConnection,
                    Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(sqlConnection),
                    b => b.MigrationsAssembly(typeof(FenixDbContext).Assembly.FullName)
                ));

            var mongoConnection = configuration.GetConnectionString("MongoDb");

            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnection));

            services.AddScoped<IMongoDatabase>(sp => {
                var client = sp.GetRequiredService<IMongoClient>();
                var databaseName = MongoUrl.Create(mongoConnection).DatabaseName;
                return client.GetDatabase(databaseName);
            });

            services.AddScoped<FenixMongoContext>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IValeRepository, ValeRepository>();
            services.AddScoped<IValeService, ValeService>();
            services.AddScoped<ISystemLogRepository, SystemLogRepository>();

            return services;
        }
    }
}
