using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Web.Services.HttpClient;

namespace Web.Services.Background
{
    public class ScopedCurrencyService : ICurrencyScopedService
    {
        private readonly BankOperationsContext _context;
        private readonly ILogger<ScopedCurrencyService> _logger;
        private readonly PrivatApiService _privatApiService;

        public ScopedCurrencyService(BankOperationsContext context, ILogger<ScopedCurrencyService> logger,
            PrivatApiService privatApiService)
        {
            _context = context;
            _logger = logger;
            _privatApiService = privatApiService;
        }
        
        public async Task DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.Log(LogLevel.Information, "Call method privat api service");

                var rate  = _context.ExchangeRate.ToList().Last();
                var value = Convert.ToDateTime($"{DateTime.Now:dd.MM.yyyy}");

                if (!rate.DateRate.Equals(value))
                {
                    var info = await _privatApiService.LoadInfo();

                    _logger.Log(LogLevel.Information, "Dtos" + info);

                    try
                    {
                        _logger.Log(LogLevel.Information, "Start saving");

                        foreach (var dto in info)
                        {
                            var firstOrDefault = _context.Currency.FirstOrDefault(currency => currency.ShortName.Equals(dto.ccy));

                            _logger.Log(LogLevel.Information, $"Currency {firstOrDefault}");

                            var exchangeRate = new ExchangeRate
                            {
                                RateBuy  = dto.buy,
                                RateSale = dto.sale,
                                DateRate = Convert.ToDateTime($"{DateTime.Now:dd.MM.yyyy}")
                            };

                            _logger.Log(LogLevel.Information, $"Rate {exchangeRate}");

                            if (firstOrDefault != null)
                            {
                                firstOrDefault.ExchangeRate.Add(exchangeRate);
                                await _context.SaveChangesAsync(cancellationToken);
                            }

                            _logger.Log(LogLevel.Information, "End saving");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("Error from service");
                        _logger.Log(LogLevel.Error, e.Message);
                        throw;
                    }
                }
                
                await Task.Delay(86_400_000, cancellationToken);
            }
        }
    }
}