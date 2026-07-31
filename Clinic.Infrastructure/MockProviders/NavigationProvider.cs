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
                new NavigationItem { Id = "dashboard", Title = "Dashboard", Icon = "bi-speedometer2", Route = "/", BreadcrumbTitle = "Dashboard", Description = "Main Dashboard", RequiredPermission = "ViewDashboard" },
                new NavigationItem { Id = "patient", Title = "Patient", Icon = "bi-person-lines-fill", Route = "/Patient", BreadcrumbTitle = "Patients", Description = "Patient Management", RequiredPermission = "ViewPatient",
                    Children = new List<NavigationItem>
                    {
                        new NavigationItem { Id = "patient-list", Title = "All Patients", Icon = "bi-people", Route = "/Patient", BreadcrumbTitle = "All Patients" },
                        new NavigationItem { Id = "patient-add", Title = "Add Patient", Icon = "bi-person-plus", Route = "/Patient/Create", BreadcrumbTitle = "Add Patient" }
                    }
                },
                new NavigationItem { Id = "appointment", Title = "Appointment", Icon = "bi-calendar-check", Route = "/Appointment", BreadcrumbTitle = "Appointments", Description = "Manage Appointments", RequiredPermission = "ViewAppointment" },
                new NavigationItem { Id = "medicalrecord", Title = "Medical Record", Icon = "bi-clipboard2-pulse", Route = "/MedicalRecord", BreadcrumbTitle = "Medical Records", Description = "Patient Medical Records", RequiredPermission = "ViewMedicalRecord" },
                new NavigationItem { Id = "billing", Title = "Billing & Payment", Icon = "bi-cash-coin", Route = "/Billing", BreadcrumbTitle = "Billing", Description = "Invoices and Payments", RequiredPermission = "ViewBilling" },
                new NavigationItem { Id = "inventory", Title = "Inventory", Icon = "bi-box-seam", Route = "/Inventory", BreadcrumbTitle = "Inventory", Description = "Stock Management", RequiredPermission = "ViewInventory" },
                new NavigationItem { Id = "report", Title = "Reports", Icon = "bi-bar-chart", Route = "/Report", BreadcrumbTitle = "Reports", Description = "Analytics and Reports", RequiredPermission = "ViewReport" },
                new NavigationItem { Id = "admin", Title = "Administration", Icon = "bi-gear", Route = "/Admin", BreadcrumbTitle = "Admin", Description = "System Administration", RequiredPermission = "ViewAdmin",
                    Children = new List<NavigationItem>
                    {
                        new NavigationItem { Id = "admin-users", Title = "Users", Icon = "bi-person", Route = "/Admin/Users", BreadcrumbTitle = "Users" },
                        new NavigationItem { Id = "admin-roles", Title = "Roles", Icon = "bi-shield-lock", Route = "/Admin/Roles", BreadcrumbTitle = "Roles" },
                        new NavigationItem { Id = "admin-locations", Title = "Locations", Icon = "bi-buildings", Route = "/Admin/Locations", BreadcrumbTitle = "Locations" },
                        new NavigationItem { Id = "admin-chairs", Title = "Chairs", Icon = "bi-display", Route = "/Admin/Chairs", BreadcrumbTitle = "Chairs" }
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
                
                foreach (var child in item.Children)
                {
                    if (child.Route.Equals(currentRoute, System.StringComparison.OrdinalIgnoreCase))
                    {
                        breadcrumbs.Add(item);
                        breadcrumbs.Add(child);
                        return breadcrumbs;
                    }
                }
            }
            
            // Default fallback
            breadcrumbs.Add(new NavigationItem { Title = "Home", Route = "/" });
            return breadcrumbs;
        }
    }
}
