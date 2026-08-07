using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Clinic.Application.Interfaces.Storage;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.Interfaces.Auth;

namespace Clinic.Web.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void ValidateEnterpriseDependencies(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Validating Enterprise Dependencies...");
                
                // File Storage Foundation
                services.GetRequiredService<IFileStorageService>();
                
                // Number Sequence Foundation
                services.GetRequiredService<INumberSequenceService>();
                
                // Master Reference Foundation
                services.GetRequiredService<IMasterReferenceService>();
                
                // Domain Services
                services.GetRequiredService<IPatientService>();
                services.GetRequiredService<IDoctorService>();
                
                // Operations
                services.GetRequiredService<Clinic.Application.Interfaces.Operations.IScheduleBoardRepository>();
                
                // Auth Foundation
                services.GetRequiredService<ICurrentUserService>();
                services.GetRequiredService<IAuthService>();
                services.GetRequiredService<IPermissionService>();

                logger.LogInformation("All Enterprise Dependencies successfully resolved at startup.");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "FATAL ERROR: Failed to resolve one or more critical enterprise dependencies during startup.");
                throw; // Stop the application
            }
        }
    }
}
