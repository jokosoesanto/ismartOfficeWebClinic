using System;
using System.Threading;
using System.Threading.Tasks;

namespace Clinic.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
