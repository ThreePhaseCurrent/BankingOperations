using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web.Commands.Deposit;
using Web.Commands.Operation;

namespace Web.Handlers.Deposit
{
    public class ApplyDepositHandler : IRequestHandler<ApplyDepositCommand, bool>
    {
        private readonly IDepositRepository _depositRepository;
        private readonly ILogger<ApplyDepositHandler> _logger;
        private readonly IMediator _mediator;
        private IBankAccountRepository _bankAccountRepository;
        
        public ApplyDepositHandler(IDepositRepository depositRepository, ILogger<ApplyDepositHandler> logger, IMediator mediator, IBankAccountRepository bankAccountRepository)
        {
            _depositRepository = depositRepository;
            _logger = logger;
            _mediator = mediator;
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<bool> Handle(ApplyDepositCommand request, CancellationToken cancellationToken)
        {
            request.Deposit.Status = true;
            request.Deposit.PercentDeposit = 1;

            var currentAccount = await _bankAccountRepository.Accounts
                    .FirstOrDefaultAsync(x => x.IdAccount == request.Deposit.IdAccount, cancellationToken);

            if (currentAccount != null && currentAccount.Amount >= request.Deposit.Amount)
            {
                currentAccount.Amount -= request.Deposit.Amount;
                await _depositRepository.AddDeposit(request.Deposit);

                await _mediator.Send(new BankAccountOperationCommand(request.Deposit.IdAccount,
                    "Передача средств на депозит",
                    request.Deposit.Amount), cancellationToken);
                _logger.Log(LogLevel.Information, $"Apply deposit in account {request.Deposit.IdAccount}.");

                return true;
            }

            return false;
        }
    }
}