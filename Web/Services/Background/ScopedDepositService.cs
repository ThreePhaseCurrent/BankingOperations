using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using BankingSystem.ApplicationCore.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web.Commands.Operation;
using Web.Interfaces;

namespace Web.Services.Background
{
    public class ScopedDepositService : IDepositScopedService
    {
        private readonly ILogger<ScopedDepositService> _logger;
        private readonly IMediator _mediator;
        private readonly IDepositRepository _depositRepository;
        private readonly IBankAccountRepository _bankAccountRepository;

        public ScopedDepositService(ILogger<ScopedDepositService> logger, IMediator mediator, 
            IDepositRepository depositRepository, IBankAccountRepository bankAccountRepository)
        {
            _logger = logger;
            _mediator = mediator;
            _depositRepository = depositRepository;
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.Log(LogLevel.Information, "Update deposit started!");
                
                //список активных депозитов
                var deposits = _depositRepository.Deposits.Where(x => x.Status).ToList();

                foreach (var deposit in deposits)
                {
                    //зачисление процентов
                    await _depositRepository.PercentAccrual(deposit.IdDeposit);
                    
                    _logger.Log(LogLevel.Information, $"Deposit {deposit.IdDeposit} is updated!");
                }
                
                //закрытие всех завершенных депозитов
                var closedDeposits = await _depositRepository.CloseFinishedDeposits();
                _logger.Log(LogLevel.Information, $"Finished deposits was closed!");

                foreach (var deposit in closedDeposits)
                {
                    //находим нужный банковский счет
                    var currentAccount = await _bankAccountRepository.Accounts
                        .FirstOrDefaultAsync(x => x.IdAccount == deposit.IdAccount, cancellationToken);
                    //происводим зачисление
                    currentAccount.Amount += deposit.Amount;
                    
                    //фиксируем зачисление средств
                    await _mediator.Send(new BankAccountOperationCommand(
                        currentAccount.IdAccount, 
                        "Зачисление", 
                        deposit.Amount), 
                        cancellationToken);
                    
                    //фиксируем завершение депозита
                    await _mediator.Send(new BankAccountOperationCommand(
                            currentAccount.IdAccount, 
                            "Успешное завершение депозита", 
                            0), 
                        cancellationToken);
                }
                
                await Task.Delay(86_400_000, cancellationToken);
            }
        }
    }
}