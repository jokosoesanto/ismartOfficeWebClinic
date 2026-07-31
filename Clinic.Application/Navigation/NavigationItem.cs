namespace Clinic.Application.Navigation
{
    public class NavigationItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequiredPermission { get; set; } = string.Empty;
        public List<NavigationItem> Children { get; set; } = new List<NavigationItem>();
        
        // Breadcrumb is generally inferred from the hierarchy, but we can store custom breadcrumb title
        public string BreadcrumbTitle { get; set; } = string.Empty;
    }
}
