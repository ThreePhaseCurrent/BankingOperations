using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using BankingSystem.ApplicationCore.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Commands.Operation;

namespace Web.Handlers.Operation
{
    /// <summary>
    ///   Обработка перевода денег
    /// </summary>
    public class TransferAmountHandler : IRequestHandler<TransferAmountCommand, bool>
    {
        private readonly IAsyncRepository<BankAccount> _accountRepository;

        private readonly IMediator                      _mediator;
        private readonly ILogger<TransferAmountHandler> _logger;

        public TransferAmountHandler(IAsyncRepository<BankAccount>  accountRepository, IMediator mediator,
            ILogger<TransferAmountHandler> logger)
        {
            _accountRepository = accountRepository;
            _mediator          = mediator;
            _logger            = logger;
            logger.LogInformation("BankingSystem.Web.Handlers.Operation.TransferAmountHandler");
        }

        public async Task<bool> Handle(TransferAmountCommand request, CancellationToken cancellationToken)
        {
            var fromAccount = await _accountRepository.GetById(request.From);
            var toAccount   = await _accountRepository.GetById(request.To);

            _logger.LogInformation($"Before sending amount: sender - {fromAccount.Amount} | receiver - {toAccount.Amount}");
            fromAccount.Amount -= request.Amount;
            toAccount.Amount   += request.Amount;
            _logger.LogInformation($"After sending amount: sender - {fromAccount.Amount} | receiver - {toAccount.Amount}");

            await _accountRepository.UpdateAsync(fromAccount);
            await _accountRepository.UpdateAsync(toAccount);

            await
                _mediator.Send(new BankAccountOperationCommand(fromAccount.IdAccount, "Перевод", request.Amount),
                    cancellationToken);
            await
                _mediator.Send(new BankAccountOperationCommand(toAccount.IdAccount, "Зачисление", request.Amount),
                    cancellationToken);
            return true;
        }
    }
}