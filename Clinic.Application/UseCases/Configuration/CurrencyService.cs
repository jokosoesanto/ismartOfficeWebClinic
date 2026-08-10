using System;
using System.Globalization;
using System.Threading.Tasks;
using Clinic.Application.Interfaces.Configuration;

namespace Clinic.Application.UseCases.Configuration
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IAppConfigurationService _configService;

        public CurrencyService(IAppConfigurationService configService)
        {
            _configService = configService;
        }

        public async Task<string> GetCurrencyCodeAsync()
        {
            var code = await _configService.GetValueAsync("ApplicationCurrency", "IDR");
            return string.IsNullOrWhiteSpace(code) ? "IDR" : code.ToUpperInvariant();
        }

        public async Task<string> GetCurrencySymbolAsync()
        {
            var code = await GetCurrencyCodeAsync();
            return GetSymbolForCode(code);
        }

        public async Task<string> FormatAmountAsync(decimal amount)
        {
            var code = await GetCurrencyCodeAsync();
            var culture = GetCultureForCode(code);
            return amount.ToString("C", culture);
        }

        private string GetSymbolForCode(string code)
        {
            return code switch
            {
                "IDR" => "Rp",
                "USD" => "$",
                "EUR" => "€",
                "SGD" => "S$",
                "MYR" => "RM",
                "AUD" => "A$",
                "JPY" => "¥",
                _ => "$"
            };
        }

        private CultureInfo GetCultureForCode(string code)
        {
            try
            {
                string cultureName = code switch
                {
                    "IDR" => "id-ID",
                    "USD" => "en-US",
                    "EUR" => "fr-FR",
                    "SGD" => "en-SG",
                    "MYR" => "ms-MY",
                    "AUD" => "en-AU",
                    "JPY" => "ja-JP",
                    _ => "en-US"
                };
                return new CultureInfo(cultureName);
            }
            catch
            {
                return new CultureInfo("en-US");
            }
        }
    }
}
