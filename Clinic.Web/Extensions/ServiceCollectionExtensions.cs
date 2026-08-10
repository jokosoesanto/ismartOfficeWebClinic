using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Clinic.Application.Interfaces.Auth;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.Interfaces.Navigation;
using Clinic.Application.Interfaces.Operations;
using Clinic.Application.Interfaces.Storage;
using Clinic.Application.UseCases.Auth;
using Clinic.Application.UseCases.Configuration;
using Clinic.Application.UseCases.MasterData;
using Clinic.Application.UseCases.Navigation;
using Clinic.Infrastructure.Data;
using Clinic.Infrastructure.Repositories.Auth;
using Clinic.Infrastructure.Repositories.Configuration;
using Clinic.Infrastructure.Repositories.MasterData;
using Clinic.Infrastructure.Repositories.Operations;
using Clinic.Infrastructure.Security;
using Clinic.Infrastructure.Storage;
using Clinic.Application.Configurations;
using Clinic.Application.Interfaces;

namespace Clinic.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<INavigationService, NavigationService>();
            
            // Master Data Services
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IChairService, ChairService>();
            services.AddScoped<ISpecialtyService, SpecialtyService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IMasterReferenceService, MasterReferenceService>();
            
            // System/Config Services
            services.AddScoped<INumberSequenceService, NumberSequenceService>();
            services.AddScoped<IAppConfigurationService, AppConfigurationService>();
            services.AddScoped<Clinic.Application.Interfaces.Configuration.ICurrencyService, Clinic.Application.UseCases.Configuration.CurrencyService>();
            services.AddScoped<ITreatmentCategoryService, TreatmentCategoryService>();
            services.AddScoped<ITreatmentSubCategoryService, Clinic.Application.Services.MasterData.TreatmentSubCategoryService>();
            services.AddScoped<ITreatmentCatalogService, Clinic.Application.Services.MasterData.TreatmentCatalogService>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Unit of Work
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());

            // Auth Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<IPasswordHasher, IdentityPasswordHasher>();

            // Master Data Repositories
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IChairRepository, ChairRepository>();
            services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IMasterReferenceRepository, MasterReferenceRepository>();
            services.AddScoped<ITreatmentCategoryRepository, TreatmentCategoryRepository>();
            services.AddScoped<ITreatmentSubCategoryRepository, TreatmentSubCategoryRepository>();
            services.AddScoped<ITreatmentCatalogRepository, TreatmentCatalogRepository>();

            // Operations Repositories
            services.AddScoped<IScheduleBoardRepository, ScheduleBoardRepository>();

            // System/Config Repositories
            services.AddScoped<INumberSequenceRepository, NumberSequenceRepository>();
            services.AddScoped<IAppConfigurationRepository, AppConfigurationRepository>();

            return services;
        }

        public static IServiceCollection AddStorageServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<StorageOptions>()
                .Bind(configuration.GetSection(StorageOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
                
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<IFileScanner, AlwaysCleanScanner>();
            
            return services;
        }

        public static IServiceCollection AddSystemFoundation(this IServiceCollection services)
        {
            // Security / Web-Specific Foundation
            services.AddScoped<ICurrentUserService, Clinic.Web.Services.CurrentUserService>();
            services.AddSingleton<IPermissionCache, Clinic.Web.Services.Auth.PermissionCache>();
            
            services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider, Clinic.Web.Security.PermissionPolicyProvider>();
            services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, Clinic.Web.Security.PermissionAuthorizationHandler>();
            
            // UI Component Foundation
            services.AddSingleton<Clinic.Application.Navigation.INavigationProvider, Clinic.Infrastructure.MockProviders.NavigationProvider>();
            services.AddSingleton<Clinic.Web.Services.Diagnostics.IComponentResolver, Clinic.Web.Services.Diagnostics.ComponentResolver>();
            services.AddSingleton<Clinic.Web.Services.Diagnostics.IComponentDiagnosticsService, Clinic.Web.Services.Diagnostics.ComponentDiagnosticsService>();
            services.AddSingleton<Clinic.Web.Services.Diagnostics.IRegistryValidatorService, Clinic.Web.Services.Diagnostics.RegistryValidatorService>();
            services.AddSingleton<Clinic.Web.Services.IComponentRegistry, Clinic.Web.Services.ComponentRegistry>();

            return services;
        }
    }
}
