using AIApp.Components;
using AIApp.Lib;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddLibraryServices();

var app = builder.Build();

app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Applying migrations...");
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<IDbContextFactory<DataContext>>();
    using var dbContext = context.CreateDbContext();
    dbContext.Database.EnsureCreated();
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}

app.Run();
