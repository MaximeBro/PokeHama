using MudBlazor.Services;
using PokeHama.Components;
using PokeHama.Extensions;
using PokeHama.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddSingleton<FetchService>();
builder.Services.AddSingleton<MiniGamesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseUploading();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


var fetchService = app.Services.GetRequiredService<FetchService>();
var minigamesService = app.Services.GetRequiredService<MiniGamesService>();
await fetchService.InitAsync();
await minigamesService.InitAsync();

await app.RunAsync();