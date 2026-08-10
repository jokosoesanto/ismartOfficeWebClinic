using System.Collections.Generic;
using Clinic.Application.Navigation;
using System.Linq;

namespace Clinic.Infrastructure.MockProviders
{
    public class NavigationProvider : INavigationProvider
    {
        private readonly List<NavigationItem> _menu;

        public NavigationProvider()
        {
            _menu = new List<NavigationItem>
            {
                new NavigationItem { Id = "dashboard", Title = "Dashboard", Icon = "bi-speedometer2", Route = "/", BreadcrumbTitle = "Dashboard", Description = "Main Dashboard" },
                new NavigationItem { Id = "patient", Title = "Patient Management", Icon = "bi-person-lines-fill", Route = "/Patient", BreadcrumbTitle = "Patients", Description = "Patient Management", RequiredPermission = "Patient.Index" },
                new NavigationItem { Id = "appointment", Title = "Appointment", Icon = "bi-calendar-check", Route = "/Appointment", BreadcrumbTitle = "Appointments", Description = "Manage Appointments", RequiredPermission = "Appointment.Index" },
                new NavigationItem { Id = "medicalrecord", Title = "Medical Record", Icon = "bi-clipboard2-pulse", Route = "/MedicalRecord", BreadcrumbTitle = "Medical Records", Description = "Patient Medical Records", RequiredPermission = "MedicalRecord.Index" },
                new NavigationItem { Id = "billing", Title = "Billing & Payment", Icon = "bi-cash-coin", Route = "/Billing", BreadcrumbTitle = "Billing", Description = "Invoices and Payments", RequiredPermission = "Billing.Index" },
                new NavigationItem { Id = "inventory", Title = "Inventory", Icon = "bi-box-seam", Route = "/Inventory", BreadcrumbTitle = "Inventory", Description = "Stock Management", RequiredPermission = "Inventory.Index" },
                new NavigationItem { Id = "report", Title = "Reports", Icon = "bi-bar-chart", Route = "/Report", BreadcrumbTitle = "Reports", Description = "Analytics and Reports", RequiredPermission = "Report.Index" },
                new NavigationItem { Id = "operations", Title = "Operations", Icon = "bi-activity", Route = "/Operations", BreadcrumbTitle = "Operations", Description = "Operational Modules", RequiredPermission = "ScheduleBoard.View",
                    Children = new List<NavigationItem>
                    {
                        new NavigationItem { Id = "schedule-board", Title = "Doctor Schedule Board", Icon = "bi-calendar2-week", Route = "/ScheduleBoard", BreadcrumbTitle = "Schedule Board", RequiredPermission = "ScheduleBoard.View" }
                    }
                },
                new NavigationItem { Id = "admin", Title = "Administration", Icon = "bi-gear", Route = "/Admin", BreadcrumbTitle = "Admin", Description = "System Administration", RequiredPermission = "Admin.Index",
                    Children = new List<NavigationItem>
                    {
                        new NavigationItem { Id = "admin-users", Title = "Users", Icon = "bi-person", Route = "/Admin/Users", BreadcrumbTitle = "Users", RequiredPermission = "Admin.Users" },
                        new NavigationItem { Id = "admin-roles", Title = "Roles", Icon = "bi-shield-lock", Route = "/Admin/Roles", BreadcrumbTitle = "Roles", RequiredPermission = "Admin.Roles" },
                        new NavigationItem { Id = "admin-locations", Title = "Locations", Icon = "bi-buildings", Route = "/Admin/Locations", BreadcrumbTitle = "Locations", RequiredPermission = "Admin.Locations" },
                        new NavigationItem { Id = "admin-chairs", Title = "Chairs", Icon = "bi-display", Route = "/Admin/Chairs", BreadcrumbTitle = "Chairs", RequiredPermission = "Admin.Chairs" }
                    }
                },
                new NavigationItem { Id = "system", Title = "System", Icon = "bi-hdd-network", Route = "/System", BreadcrumbTitle = "System", Description = "System Configuration", RequiredPermission = "MasterReference.Index",
                    Children = new List<NavigationItem>
                    {
                        new NavigationItem { Id = "system-master-reference", Title = "Master References", Icon = "bi-journal-text", Route = "/MasterReference", BreadcrumbTitle = "Master References", RequiredPermission = "MasterReference.Index" },
                        new NavigationItem { Id = "system-number-sequence", Title = "Number Sequences", Icon = "bi-123", Route = "/NumberSequence", BreadcrumbTitle = "Number Sequences", RequiredPermission = "NumberSequence.Index" },
                        new NavigationItem { 
                            Id = "system-treatment-management", 
                            Title = "Treatment Management", 
                            Icon = "bi-journal-medical", 
                            Route = "/TreatmentManagement", 
                            BreadcrumbTitle = "Treatment Management", 
                            RequiredPermission = "MasterData.TreatmentCategory.View",
                            Children = new List<NavigationItem>
                            {
                                new NavigationItem { Id = "system-treatment-category", Title = "Treatment Categories", Icon = "bi-tags", Route = "/TreatmentCategory", BreadcrumbTitle = "Treatment Categories", RequiredPermission = "MasterData.TreatmentCategory.View" },
                                new NavigationItem { Id = "system-treatment-subcategory", Title = "Treatment SubCategories", Icon = "bi-tag", Route = "/TreatmentSubCategory", BreadcrumbTitle = "Treatment SubCategories", RequiredPermission = "MasterData.TreatmentSubCategory.View" },
                                new NavigationItem { Id = "system-treatment-catalog", Title = "Treatment Catalog", Icon = "bi-card-list", Route = "/TreatmentCatalog", BreadcrumbTitle = "Treatment Catalog", RequiredPermission = "MasterData.TreatmentCatalog.View" }
                            }
                        },
                        new NavigationItem { Id = "system-currency", Title = "Application Currency", Icon = "bi-currency-exchange", Route = "/Configuration/Currency", BreadcrumbTitle = "Application Currency", RequiredPermission = "Configuration.Currency" }
                    }
                }
            };
        }

        public List<NavigationItem> GetNavigationMenu()
        {
            return _menu;
        }

        public List<NavigationItem> GetBreadcrumbs(string currentRoute)
        {
            var breadcrumbs = new List<NavigationItem>();
            
            // Simple logic to find breadcrumb by route matching
            foreach (var item in _menu)
            {
                if (item.Route.Equals(currentRoute, System.StringComparison.OrdinalIgnoreCase))
                {
                    breadcrumbs.Add(item);
                    return breadcrumbs;
                }
                
                if (item.Children == null) continue;
                
                foreach (var child in item.Children)
                {
                    if (child.Route.Equals(currentRoute, System.StringComparison.OrdinalIgnoreCase))
                    {
                        breadcrumbs.Add(item);
                        breadcrumbs.Add(child);
                        return breadcrumbs;
                    }
                    
                    if (child.Children == null) continue;
                    
                    foreach (var grandChild in child.Children)
                    {
                        if (grandChild.Route.Equals(currentRoute, System.StringComparison.OrdinalIgnoreCase))
                        {
                            breadcrumbs.Add(item);
                            breadcrumbs.Add(child);
                            breadcrumbs.Add(grandChild);
                            return breadcrumbs;
                        }
                    }
                }
            }
            
            // Default fallback
            breadcrumbs.Add(new NavigationItem { Title = "Home", Route = "/" });
            return breadcrumbs;
        }
    }
}
