using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using PokeHama.Components;
using PokeHama.Databases;
using PokeHama.Extensions;
using PokeHama.Services;

var builder = WebApplication.CreateBuilder(args);

var pathToData = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "../data/")).FullName;

/* Authentication (start) */
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "auth_token";
    options.LoginPath = "/connexion";
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
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
builder.Services.AddSingleton<FetchService>();
builder.Services.AddSingleton<MiniGamesService>();
builder.Services.AddSingleton<UserService>();
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

app.MapControllers();
app.UseUploading(); // Used to send to client files stored on the server-side

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await RunMigrationAsync<UtilityContext>(app);

var fetchService = app.Services.GetRequiredService<FetchService>();
var minigamesService = app.Services.GetRequiredService<MiniGamesService>();
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