using System.Threading.Tasks;

namespace Clinic.Application.Interfaces.Configuration
{
    public interface ICurrencyService
    {
        Task<string> GetCurrencyCodeAsync();
        Task<string> GetCurrencySymbolAsync();
        Task<string> FormatAmountAsync(decimal amount);
    }
}
