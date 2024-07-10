using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using PokeHama.Components;
using PokeHama.Databases;
using PokeHama.Extensions;
using PokeHama.Services;

var builder = WebApplication.CreateBuilder(args);

var pathToData = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "../data/")).FullName;
var pathToSettings = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "../data/settings/")).FullName;

builder.Configuration.AddJsonFile(Path.Combine(pathToSettings, "users.json"), optional: false, reloadOnChange: true);

/* Authentication (start) */
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "auth_token";
    options.LoginPath = "/connexion";
});
builder.Services.AddHttpContextAccessor();
/* Authentication (end) */

/* Databases (start) */
builder.Services.AddDbContextFactory<UtilityContext>(options =>
{
    options.UseSqlite($"Data source={Path.Combine(pathToData, "db/utility.db")}");
});
/* Databases (end) */

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

/* Custom Services (start) */
builder.Services.AddSingleton<StartupService>();
builder.Services.AddSingleton<FetchService>();
builder.Services.AddSingleton<MiniGamesService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<UserTokenService>();
builder.Services.AddScoped<RelationshipManager>();
builder.Services.AddScoped<AuthenticationService>();
/*Custom services (end) */

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
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict});

app.UseLogin(); // Used as a mapped controller to manage user authentication
app.UseLogout(); // Used as a mapped controller to manage user authentication
app.UseUploading(); // Used to send to client files stored on the server-side

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await RunMigrationAsync<UtilityContext>(app);

var startupService = app.Services.GetRequiredService<StartupService>();
var fetchService = app.Services.GetRequiredService<FetchService>();
var minigamesService = app.Services.GetRequiredService<MiniGamesService>();
await startupService.InitAsync();
await fetchService.InitAsync();
await minigamesService.InitAsync();

await app.RunAsync();
return;

async Task RunMigrationAsync<T>(IHost webApp) where T : DbContext
{
    var factory = webApp.Services.GetRequiredService<IDbContextFactory<T>>();
    var db = await factory.CreateDbContextAsync();
    await db.Database.MigrateAsync();
    await db.DisposeAsync();
}