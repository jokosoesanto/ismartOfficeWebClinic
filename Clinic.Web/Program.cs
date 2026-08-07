using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Clinic.Infrastructure.Data;
using Clinic.Infrastructure.Data.Seeders;
using Clinic.Application.Interfaces.Auth;
using Clinic.Application.UseCases.Auth;
using Clinic.Infrastructure.Repositories.Auth;
using Clinic.Infrastructure.Security;
using Clinic.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString, b => b.MigrationsAssembly("Clinic.Infrastructure")));

// Enterprise Services
builder.Services.AddSystemFoundation();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddStorageServices(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    });

var app = builder.Build();

app.ValidateEnterpriseDependencies();

// Automatically apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    var retryCount = 3;
    var delay = TimeSpan.FromSeconds(2);
    
    for (int i = 0; i < retryCount; i++)
    {
        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync();
            }
            
            AuthSeeder.Seed(context);
            SystemSeeder.Seed(context);
            await Clinic.Web.Services.PermissionSynchronizer.SyncAsync(context, typeof(Program).Assembly, app.Logger);
            break; // Success
        }
        catch (Exception ex)
        {
            var sqliteEx = ex as Microsoft.Data.Sqlite.SqliteException ?? ex.InnerException as Microsoft.Data.Sqlite.SqliteException;
            if (sqliteEx != null && sqliteEx.SqliteErrorCode == 5)
            {
                if (i == retryCount - 1)
                {
                    app.Logger.LogCritical(ex, "SQLite database is locked and could not be accessed after multiple retries.");
                    throw; // Rethrow on last attempt
                }
                app.Logger.LogWarning($"SQLite database is locked. Retrying in {delay.TotalSeconds} seconds...");
                await Task.Delay(delay);
                delay *= 2; // Exponential backoff
            }
            else
            {
                throw; // Rethrow if it's not a lock error
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

