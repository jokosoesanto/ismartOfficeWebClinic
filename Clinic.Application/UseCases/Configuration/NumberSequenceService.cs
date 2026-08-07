using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Domain.Entities.System;
using Clinic.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.UseCases.Configuration
{
    public class NumberSequenceService : INumberSequenceService
    {
        private readonly INumberSequenceRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NumberSequenceService> _logger;

        public NumberSequenceService(INumberSequenceRepository repository, IUnitOfWork unitOfWork, ILogger<NumberSequenceService> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<NumberSequence>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllAsync(cancellationToken);
        }

        public async Task<string> GenerateSequenceAsync(string sequenceCode, CancellationToken cancellationToken = default)
        {
            var maxRetries = 5;
            var delay = TimeSpan.FromMilliseconds(50);

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var sequence = await _repository.GetByCodeAsync(sequenceCode, cancellationToken);
                    if (sequence == null)
                    {
                        throw new InvalidOperationException($"NumberSequence with code '{sequenceCode}' not found.");
                    }

                    string currentDateStr = GetCurrentDateString(sequence.DatePattern);

                    if (ShouldReset(sequence))
                    {
                        sequence.CurrentValue = sequence.IncrementStep;
                        sequence.LastDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        sequence.CurrentValue += sequence.IncrementStep;
                    }

                    sequence.RowVersion = Guid.NewGuid(); // Update concurrency token
                    
                    await _repository.UpdateAsync(sequence, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return FormatSequence(sequence, currentDateStr);
                }
                catch (Exception ex) when (ex.GetType().Name.Contains("DbUpdateConcurrencyException"))
                {
                    if (i == maxRetries - 1)
                    {
                        _logger.LogError(ex, $"Failed to generate sequence for {sequenceCode} after {maxRetries} attempts due to concurrency.");
                        throw;
                    }
                    
                    _repository.DetachAll();
                    
                    await Task.Delay(delay, cancellationToken);
                    delay *= 2; // Exponential backoff
                }
            }

            throw new InvalidOperationException("Failed to generate sequence.");
        }

        public async Task<string> PreviewNextNumberAsync(string sequenceCode, CancellationToken cancellationToken = default)
        {
            var sequence = await _repository.GetByCodeAsync(sequenceCode, cancellationToken);
            if (sequence == null)
            {
                throw new InvalidOperationException($"NumberSequence with code '{sequenceCode}' not found.");
            }
            
            // Immediately detach so we don't accidentally save it
            _repository.DetachAll();

            string currentDateStr = GetCurrentDateString(sequence.DatePattern);
            long nextValue = sequence.CurrentValue;

            if (ShouldReset(sequence))
            {
                nextValue = sequence.IncrementStep;
            }
            else
            {
                nextValue += sequence.IncrementStep;
            }

            var previewSeq = new NumberSequence
            {
                Prefix = sequence.Prefix,
                DatePattern = sequence.DatePattern,
                Padding = sequence.Padding,
                CurrentValue = nextValue
            };

            return FormatSequence(previewSeq, currentDateStr);
        }

        private string GetCurrentDateString(string? datePattern)
        {
            if (string.IsNullOrWhiteSpace(datePattern)) return string.Empty;
            try
            {
                return DateTime.UtcNow.ToString(datePattern);
            }
            catch
            {
                return DateTime.UtcNow.ToString("yyyyMMdd");
            }
        }

        private bool ShouldReset(NumberSequence sequence)
        {
            if (sequence.ResetPolicy == SequenceResetPolicy.Never) return false;
            
            if (string.IsNullOrEmpty(sequence.LastDate)) return true;

            var now = DateTime.UtcNow;
            
            DateTime lastParsed;
            if (!DateTime.TryParseExact(sequence.LastDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out lastParsed))
            {
                return true; 
            }

            return sequence.ResetPolicy switch
            {
                SequenceResetPolicy.Daily => now.Date > lastParsed.Date,
                SequenceResetPolicy.Monthly => now.Year > lastParsed.Year || now.Month > lastParsed.Month,
                SequenceResetPolicy.Yearly => now.Year > lastParsed.Year,
                _ => false
            };
        }

        private string FormatSequence(NumberSequence sequence, string formattedDate)
        {
            string numberPart = sequence.CurrentValue.ToString().PadLeft(sequence.Padding, '0');
            return $"{sequence.Prefix}{formattedDate}{numberPart}";
        }
    }
}
