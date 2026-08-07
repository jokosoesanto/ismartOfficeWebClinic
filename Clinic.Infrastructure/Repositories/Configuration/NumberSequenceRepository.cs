using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Domain.Entities.System;
using Clinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Repositories.Configuration
{
    public class NumberSequenceRepository : INumberSequenceRepository
    {
        private readonly AppDbContext _context;

        public NumberSequenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<System.Collections.Generic.IEnumerable<NumberSequence>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.NumberSequences.ToListAsync(cancellationToken);
        }

        public async Task<NumberSequence?> GetByCodeAsync(string sequenceCode, CancellationToken cancellationToken = default)
        {
            return await _context.NumberSequences
                .FirstOrDefaultAsync(x => x.SequenceCode == sequenceCode, cancellationToken);
        }

        public Task UpdateAsync(NumberSequence sequence, CancellationToken cancellationToken = default)
        {
            _context.NumberSequences.Update(sequence);
            return Task.CompletedTask;
        }

        public void DetachAll()
        {
            var entries = _context.ChangeTracker.Entries<NumberSequence>().ToList();
            foreach (var entry in entries)
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}
