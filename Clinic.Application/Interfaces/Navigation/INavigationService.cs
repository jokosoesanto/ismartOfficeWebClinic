using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Clinic.Application.Navigation;

namespace Clinic.Application.Interfaces.Navigation
{
    public interface INavigationService
    {
        Task<List<NavigationItem>> GetAuthorizedMenuAsync(Guid userId, bool isAdmin);
    }
}
