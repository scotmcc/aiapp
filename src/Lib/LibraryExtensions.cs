using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using OllamaSharp;
using AIApp.Lib.Interfaces;
using AIApp.Lib.Services;
using Microsoft.EntityFrameworkCore.Design;

namespace AIApp.Lib
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            string connectionString = $"Host=localhost;Port=5432;Database=aiapp;Username=aiapp;Password=aiapp";
            optionsBuilder.UseNpgsql(connectionString);
            return new DataContext(optionsBuilder.Options);
        }
    }
    public static class LibraryExtensions
    {
        public static IServiceCollection AddLibraryServices(this IServiceCollection services)
        {
            var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ??
                throw new InvalidOperationException("REDIS_CONNECTION_STRING environment variable is not set.");
            var ollamaApiUrl = Environment.GetEnvironmentVariable("OLLAMA_API_URL") ??
                throw new InvalidOperationException("OLLAMA_API_URL environment variable is not set.");
            var ollamaBaseModel = Environment.GetEnvironmentVariable("OLLAMA_CHAT_MODEL") ??
                throw new InvalidOperationException("OLLAMA_CHAT_MODEL environment variable is not set.");
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
            services.AddScoped<IVoiceService, VoiceService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IMemoryService, MemoryService>();
            return services;
        }
    }
    public static class DataExtensions
    {
        public static string Ago(this DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;
            if (timeSpan.TotalSeconds < 15)
                return "Just now";
            if (timeSpan.TotalSeconds < 60)
                return $"{timeSpan.Seconds} seconds ago";
            if (timeSpan.TotalMinutes < 60)
                return $"{timeSpan.Minutes} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{timeSpan.Hours} hours ago";
            if (timeSpan.TotalDays < 30)
                return $"{timeSpan.Days} days ago";
            if (timeSpan.TotalDays < 365)
                return $"{timeSpan.Days / 30} months ago";
            return $"{timeSpan.Days / 365} years ago";
        }
        public static string ToDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}