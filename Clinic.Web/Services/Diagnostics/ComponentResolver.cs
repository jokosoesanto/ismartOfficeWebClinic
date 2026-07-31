using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Clinic.Web.Services.Diagnostics;

namespace Clinic.Web.Services.Diagnostics
{
    public interface IComponentResolver
    {
        ComponentManifest ResolveComponent(string componentId);
        IEnumerable<ComponentManifest> GetManifest();
    }

    public class ComponentResolver : IComponentResolver
    {
        private readonly IViewComponentDescriptorCollectionProvider _descriptorProvider;

        public ComponentResolver(IViewComponentDescriptorCollectionProvider descriptorProvider)
        {
            _descriptorProvider = descriptorProvider;
        }

        public ComponentManifest ResolveComponent(string componentId)
        {
            var descriptors = _descriptorProvider.ViewComponents.Items
                .Where(x => string.Equals(x.ShortName, componentId, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            var manifest = new ComponentManifest
            {
                ComponentId = componentId,
                ExpectedViewPath = $"Views/Shared/Components/{componentId}/Default.cshtml"
            };

            if (descriptors.Count == 0)
            {
                manifest.Status = ComponentResolutionStatus.Missing;
                manifest.RegistrationSource = "Unknown";
                return manifest;
            }

            if (descriptors.Count > 1)
            {
                manifest.Status = ComponentResolutionStatus.Duplicate;
                manifest.RegistrationSource = "Multiple Assemblies";
                return manifest;
            }

            var descriptor = descriptors.First();
            manifest.Namespace = descriptor.TypeInfo.Namespace ?? string.Empty;
            manifest.Assembly = descriptor.TypeInfo.Assembly.GetName().Name ?? string.Empty;
            manifest.RegistrationSource = "ApplicationPartManager";

            if (string.IsNullOrEmpty(manifest.Namespace) || !manifest.Namespace.Contains("Clinic"))
            {
                manifest.Status = ComponentResolutionStatus.InvalidNamespace;
            }
            else if (string.IsNullOrEmpty(manifest.Assembly))
            {
                manifest.Status = ComponentResolutionStatus.MissingAssembly;
            }
            // Realistically, to validate "MissingView", we'd need ICompositeViewEngine, 
            // but for a lightweight resolver we assume registered components that pass type checks are OK unless tested at rendering.
            else
            {
                manifest.Status = ComponentResolutionStatus.Registered;
            }

            return manifest;
        }

        public IEnumerable<ComponentManifest> GetManifest()
        {
            var descriptors = _descriptorProvider.ViewComponents.Items;
            return descriptors.Select(d => new ComponentManifest
            {
                ComponentId = d.ShortName,
                Namespace = d.TypeInfo.Namespace ?? string.Empty,
                Assembly = d.TypeInfo.Assembly.GetName().Name ?? string.Empty,
                RegistrationSource = "ApplicationPartManager",
                Status = ComponentResolutionStatus.Registered,
                ExpectedViewPath = $"Views/Shared/Components/{d.ShortName}/Default.cshtml"
            });
        }
    }
}
