namespace Clinic.Web.Services.Diagnostics
{
    public enum ComponentResolutionStatus
    {
        Registered,
        Missing,
        Duplicate,
        MissingView,
        MissingAssembly,
        InvalidNamespace,
        InvalidViewComponent
    }

    public class ComponentManifest
    {
        public string ComponentId { get; set; } = string.Empty;
        public string Namespace { get; set; } = string.Empty;
        public string Assembly { get; set; } = string.Empty;
        public string ExpectedViewPath { get; set; } = string.Empty;
        public ComponentResolutionStatus Status { get; set; }
        public string RegistrationSource { get; set; } = string.Empty;
        
        // Optional context info when manifested from a screen/metadata
        public string ExpectedRegion { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
    }
}
