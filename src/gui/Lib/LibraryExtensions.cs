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
            var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost:6379";
            var ollamaApiUrl = Environment.GetEnvironmentVariable("OLLAMA_API_URL") ?? "http://mac.bigeye-goblin.ts.net:11434";
            services.AddSingleton(ConnectionMultiplexer.Connect(redisConnectionString));
            services.AddScoped(_ => new OllamaApiClient(ollamaApiUrl, "llama3.2:3b"));
            services.AddSingleton<IRedisService, RedisService>();
            services.AddHttpClient<IApiService, ApiService>(client =>
            {
                var baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "https://aiapp.bigeye-goblin.ts.net/api/";
                client.BaseAddress = new Uri(baseUrl);
            });
            return services;
        }

    }
}