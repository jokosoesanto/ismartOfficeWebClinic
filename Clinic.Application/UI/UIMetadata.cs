namespace Clinic.Application.UI
{
    public enum RenderingMode
    {
        Metadata,
        Template
    }

    public class UIComposition
    {
        public List<UIComponent> North { get; set; } = new();
        public List<UIComponent> West { get; set; } = new();
        public List<UIComponent> Center { get; set; } = new();
        public List<UIComponent> East { get; set; } = new();
        public List<UIComponent> South { get; set; } = new();
    }

    public class UIComponent
    {
        public string ComponentId { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
        public bool IsLazy { get; set; } = false;
        public string LazyLoadUrl { get; set; } = string.Empty;
    }

    public class UIMetadata
    {
        public string Title { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public RenderingMode Mode { get; set; } = RenderingMode.Metadata;
        public UIComposition Composition { get; set; } = new();
        
        // Context specific data injected into components
        public object? Data { get; set; } 
    }
}
