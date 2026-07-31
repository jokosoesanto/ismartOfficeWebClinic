using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Clinic.Application.UI;

namespace Clinic.Web.Services.Diagnostics
{
    public interface IRegistryValidatorService
    {
        RegistryCoverageReport GenerateReport();
    }

    public class RegistryCoverageReport
    {
        public List<UIMetadata> ValidMetadata { get; set; } = new();
        public List<(UIMetadata Metadata, string ComponentId)> MissingComponentMetadata { get; set; } = new();
        public List<ComponentManifest> UnusedComponents { get; set; } = new();
        public List<ComponentManifest> DuplicateRegistrations { get; set; } = new();
    }

    public class RegistryValidatorService : IRegistryValidatorService
    {
        private readonly IComponentDiagnosticsService _diagnosticsService;
        private readonly IComponentResolver _resolver;

        public RegistryValidatorService(IComponentDiagnosticsService diagnosticsService, IComponentResolver resolver)
        {
            _diagnosticsService = diagnosticsService;
            _resolver = resolver;
        }

        public RegistryCoverageReport GenerateReport()
        {
            var report = new RegistryCoverageReport();
            var allComponents = _diagnosticsService.GetAllComponents().ToList();
            
            report.DuplicateRegistrations = allComponents
                .GroupBy(x => x.ComponentId)
                .Where(g => g.Count() > 1)
                .Select(g => g.First())
                .ToList();

            var usedComponentIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Dynamically scan all controllers for parameterless HttpGet methods returning ViewResult with UIMetadata
            var controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(Controller).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var controller in controllers)
            {
                var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(m => m.GetParameters().Length == 0 && m.ReturnType == typeof(IActionResult));

                foreach (var method in methods)
                {
                    try
                    {
                        var instance = Activator.CreateInstance(controller) as Controller;
                        if (instance != null)
                        {
                            var result = method.Invoke(instance, null) as ViewResult;
                            if (result?.Model is UIMetadata meta)
                            {
                                var validation = _diagnosticsService.ValidateMetadata(meta);
                                if (validation.IsValid)
                                {
                                    report.ValidMetadata.Add(meta);
                                }
                                else
                                {
                                    foreach (var missing in validation.MissingComponents)
                                    {
                                        report.MissingComponentMetadata.Add((meta, missing.ComponentId));
                                    }
                                }

                                // track usage
                                foreach (var valid in validation.ValidComponents)
                                {
                                    usedComponentIds.Add(valid.ComponentId);
                                }
                                foreach (var missing in validation.MissingComponents)
                                {
                                    usedComponentIds.Add(missing.ComponentId);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Ignore controllers that can't be instantiated parameterlessly or methods that throw
                    }
                }
            }

            // Find unused components
            report.UnusedComponents = allComponents
                .Where(c => !usedComponentIds.Contains(c.ComponentId))
                .ToList();

            return report;
        }
    }
}
