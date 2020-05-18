using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Web.Services.Background
{
    public class CurrencyScopedServiceHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private ILogger<CurrencyScopedServiceHostedService> _logger;

        public CurrencyScopedServiceHostedService(IServiceProvider serviceProvider, ILogger<CurrencyScopedServiceHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service running.");
            
            using var scope = _serviceProvider.CreateScope();
            var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ICurrencyScopedService>();
            await scopedProcessingService.DoWork(cancellationToken);
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service running.");
            
            await DoWork(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            await Task.CompletedTask;
        }
    }
}