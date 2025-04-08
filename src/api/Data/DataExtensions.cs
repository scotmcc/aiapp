using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public static class DataExtensions
    {
        public static void AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            string Database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "postgres";
            string Username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
            string Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";
            services.AddAutoMapper(typeof(DataMapping).Assembly);
            services.AddDbContextFactory<DataContext>(options =>
            {
                string connectionString = $"Host=localhost;Port=5432;Database={Database};Username={Username};Password={Password}";
                options.UseNpgsql(connectionString);
            });
        }
    }
}