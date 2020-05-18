using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Interfaces;

namespace Web.Services.Background
{
    public class DepositScopedServiceHostedService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public DepositScopedServiceHostedService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope                   = _services.CreateScope();
            var       scopedProcessingService = scope.ServiceProvider.GetRequiredService<IDepositScopedService>();

            await scopedProcessingService.DoWork(stoppingToken).ConfigureAwait(continueOnCapturedContext: true);
        }
    }
}