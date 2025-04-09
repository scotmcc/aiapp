using StackExchange.Redis;
using OllamaSharp;
using UI.Lib.Interfaces;
using UI.Lib.Services;

namespace UI.Lib
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