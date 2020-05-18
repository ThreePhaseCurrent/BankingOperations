using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using BankingSystem.ApplicationCore.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Commands.Operation;

namespace Web.Handlers.Operation
{
    public class TransferValidationHandler : IRequestHandler<TransferValidationCommand, bool>
    {
        private readonly IAsyncRepository<BankAccount>      _accountRepository;
        private readonly ILogger<TransferValidationHandler> _logger;

        public TransferValidationHandler(IAsyncRepository<BankAccount>      accountRepository,
            ILogger<TransferValidationHandler> logger)
        {
            _accountRepository = accountRepository;
            _logger            = logger;
        }

        /// <summary>
        ///   Validate account possible to change currency
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(TransferValidationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Transfer validation handler call.\nCheck account information for accountId: {request.AccountId}");
            var account = await _accountRepository.GetById(request.AccountId);
            var valid   = account.Credit.Any() == false && account.Deposit.Any() == false && account.Amount > 0;
            _logger.LogInformation($"If account can transfer: {valid}");
            return valid;
        }
    }
}