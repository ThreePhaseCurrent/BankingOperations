using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ApplicationCore.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Web.Commands.Credit;
using Web.Commands.Operation;

namespace Web.Handlers.Credit
{
    public class ApplyCreditHandler : IRequestHandler<ApplyCreditCommand, bool>
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ICreditRepository _creditRepository;
        private readonly ILogger<ApplyCreditHandler> _logger;
        private readonly IMediator _mediator;

        public ApplyCreditHandler(IBankAccountRepository bankAccountRepository,
                                  ICreditRepository creditRepository,
                                  IMediator mediator,
                                  ILogger<ApplyCreditHandler> logger)
        {
            _bankAccountRepository = bankAccountRepository;
            _creditRepository = creditRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<bool> Handle(ApplyCreditCommand request, CancellationToken token)
        {
            var credit = request.Credit;
            credit.PercentCredit = 1;
            credit.Status = true;

            await _creditRepository.AddCredit(credit);

            //счет, на который нужно перечислить средства
            var currentAccount = await _bankAccountRepository.Accounts.FirstAsync(account => account.IdAccount == credit.IdAccount);

            if(currentAccount != null)
            {
                currentAccount.Amount += credit.Amount;
                await _bankAccountRepository.SaveAccount(currentAccount);

                await _mediator.Send(new BankAccountOperationCommand(credit.IdAccount, "Зачисление суммы кредита",
                                                                     credit.Amount));
                _logger.Log(LogLevel.Information, $"Account: {credit.IdAccount} Loan amount credited to the account");

                return true;
            }

            return false;
        }
    }
}
