using System.Threading.Tasks;
using Clinic.Domain.Entities.Auth;

namespace Clinic.Application.Interfaces.Auth
{
    public interface IAuditRepository
    {
        Task AddAsync(AuditLog log);
    }
}
