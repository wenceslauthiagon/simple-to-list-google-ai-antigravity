using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoList.Application.Interfaces;
using TodoList.Infrastructure.Configuration;
using TodoList.Infrastructure.Persistence.Repositories;

namespace TodoList.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var connectionString = string.IsNullOrWhiteSpace(settings.ConnectionString)
                ? "mongodb://localhost:27017"
                : settings.ConnectionString;
            return new MongoClient(connectionString);
        });

        services.AddScoped(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            var databaseName = string.IsNullOrWhiteSpace(settings.DatabaseName)
                ? "TodoListDb"
                : settings.DatabaseName;
            return client.GetDatabase(databaseName);
        });

        services.AddScoped<ITaskRepository, MongoTaskRepository>();

        return services;
    }
}
