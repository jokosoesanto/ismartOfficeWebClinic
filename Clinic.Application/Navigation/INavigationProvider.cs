using System.Collections.Generic;

namespace Clinic.Application.Navigation
{
    public interface INavigationProvider
    {
        List<NavigationItem> GetNavigationMenu();
        List<NavigationItem> GetBreadcrumbs(string currentRoute);
    }
}
