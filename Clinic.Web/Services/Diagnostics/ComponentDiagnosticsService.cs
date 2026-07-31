using System.Collections.Generic;
using System.Linq;
using Clinic.Application.UI;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Clinic.Web.Services.Diagnostics
{
    public interface IComponentDiagnosticsService
    {
        IEnumerable<ComponentManifest> GetAllComponents();
        RegistryValidationReport ValidateMetadata(UIMetadata metadata);
    }

    public class RegistryValidationReport
    {
        public bool IsValid { get; set; }
        public List<ComponentManifest> MissingComponents { get; set; } = new();
        public List<ComponentManifest> ValidComponents { get; set; } = new();
    }

    public class ComponentDiagnosticsService : IComponentDiagnosticsService
    {
        private readonly IComponentResolver _resolver;

        public ComponentDiagnosticsService(IComponentResolver resolver)
        {
            _resolver = resolver;
        }

        public IEnumerable<ComponentManifest> GetAllComponents()
        {
            return _resolver.GetManifest();
        }

        public RegistryValidationReport ValidateMetadata(UIMetadata metadata)
        {
            var report = new RegistryValidationReport { IsValid = true };
            if (metadata == null || metadata.Composition == null)
            {
                report.IsValid = false;
                return report;
            }

            var allRequestedComponents = new List<(string Region, UIComponent Component)>();
            allRequestedComponents.AddRange(metadata.Composition.North.Select(c => ("North", c)));
            allRequestedComponents.AddRange(metadata.Composition.West.Select(c => ("West", c)));
            allRequestedComponents.AddRange(metadata.Composition.Center.Select(c => ("Center", c)));
            allRequestedComponents.AddRange(metadata.Composition.East.Select(c => ("East", c)));
            allRequestedComponents.AddRange(metadata.Composition.South.Select(c => ("South", c)));

            foreach (var req in allRequestedComponents)
            {
                var manifest = _resolver.ResolveComponent(req.Component.ComponentId);
                manifest.ExpectedRegion = req.Region;

                if (manifest.Status != ComponentResolutionStatus.Registered)
                {
                    report.IsValid = false;
                    report.MissingComponents.Add(manifest);
                }
                else
                {
                    report.ValidComponents.Add(manifest);
                }
            }

            return report;
        }
    }
}
