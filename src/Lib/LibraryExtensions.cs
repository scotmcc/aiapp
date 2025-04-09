using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using OllamaSharp;
using AIApp.Lib.Interfaces;
using AIApp.Lib.Services;

namespace AIApp.Lib
{
    public static class LibraryExtensions
    {
        public static IServiceCollection AddLibraryServices(this IServiceCollection services)
        {
            var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ??
                throw new InvalidOperationException("REDIS_CONNECTION_STRING environment variable is not set.");
            var ollamaApiUrl = Environment.GetEnvironmentVariable("OLLAMA_API_URL") ??
                throw new InvalidOperationException("OLLAMA_API_URL environment variable is not set.");
            var ollamaBaseModel = Environment.GetEnvironmentVariable("OLLAMA_BASE_MODEL") ??
                throw new InvalidOperationException("OLLAMA_BASE_MODEL environment variable is not set.");
            var apiBaseURl = Environment.GetEnvironmentVariable("API_BASE_URL") ??
                throw new InvalidOperationException("API_BASE_URL environment variable is not set.");
            string Database = Environment.GetEnvironmentVariable("POSTGRES_DB") ??
                throw new InvalidOperationException("POSTGRES_DB environment variable is not set.");
            string Username = Environment.GetEnvironmentVariable("POSTGRES_USER") ??
                throw new InvalidOperationException("POSTGRES_USER environment variable is not set.");
            string Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ??
                throw new InvalidOperationException("POSTGRES_PASSWORD environment variable is not set.");
            services.AddAutoMapper(typeof(DataMapping).Assembly);
            services.AddDbContextFactory<DataContext>(options =>
            {
                string connectionString = $"Host=localhost;Port=5432;Database={Database};Username={Username};Password={Password}";
                options.UseNpgsql(connectionString);
            });
            services.AddSingleton(ConnectionMultiplexer.Connect(redisConnectionString));
            services.AddSingleton(_ => new OllamaApiClient(ollamaApiUrl, ollamaBaseModel));
            services.AddSingleton<IRedisService, RedisService>();
            services.AddHttpClient<IApiService, ApiService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseURl);
            });
            return services;
        }
    }
}