var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Clinic.Application.Navigation.INavigationProvider, Clinic.Infrastructure.MockProviders.NavigationProvider>();
// Component Services
builder.Services.AddSingleton<Clinic.Web.Services.Diagnostics.IComponentResolver, Clinic.Web.Services.Diagnostics.ComponentResolver>();
builder.Services.AddSingleton<Clinic.Web.Services.Diagnostics.IComponentDiagnosticsService, Clinic.Web.Services.Diagnostics.ComponentDiagnosticsService>();
builder.Services.AddSingleton<Clinic.Web.Services.Diagnostics.IRegistryValidatorService, Clinic.Web.Services.Diagnostics.RegistryValidatorService>();
builder.Services.AddSingleton<Clinic.Web.Services.IComponentRegistry, Clinic.Web.Services.ComponentRegistry>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
