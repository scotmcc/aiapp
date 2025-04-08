using UI.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddHttpClient("ApiClient", client =>
{
    var BaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "https://aiapp.bigeye-goblin.ts.net/api/";
    client.BaseAddress = new Uri(BaseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
