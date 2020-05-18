using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Interfaces;

namespace Web.Services.Background
{
    public class CreditScopedServiceHostedService : BackgroundService
    {
        private readonly IServiceProvider _services;
        public CreditScopedServiceHostedService(IServiceProvider services) => _services = services;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await DoWork(stoppingToken);

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using var scope                   = _services.CreateScope();
            var       scopedProcessingService = scope.ServiceProvider.GetRequiredService<ICreditScopedService>();

            await scopedProcessingService.DoWork(stoppingToken).ConfigureAwait(continueOnCapturedContext: true);
        }

        public override async Task StopAsync(CancellationToken stoppingToken) => await Task.CompletedTask;
    }
}