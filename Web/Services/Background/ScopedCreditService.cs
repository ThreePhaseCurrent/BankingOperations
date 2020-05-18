using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web.Commands.Operation;
using Web.Interfaces;

namespace Web.Services.Background
{
    public class ScopedCreditService : ICreditScopedService
    {
        private readonly BankOperationsContext _context;
        private readonly ILogger<ScopedCreditService> _logger;
        private readonly IMediator _mediator;

        public ScopedCreditService(BankOperationsContext context,
            ILogger<ScopedCreditService> logger,
            IMediator mediator,
            IBankAccountRepository bankAccountRepository)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        private async Task<decimal> CalcPercentAmount(Credit credit)
        {
            var startCredit = await _context.Operation
                .OrderByDescending(x => x.OperationTime)
                .FirstAsync(x => x.TypeOperation == "Зачисление суммы кредита");

            return startCredit.Amount * credit.PercentCredit / 100;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.Log(logLevel: LogLevel.Information, message: $"Update credit starting! {DateTime.Now}");

                //все активные кредиты
                var credits = _context.Credit.Where(predicate: c => c.Status).ToList();

                foreach (var credit in credits)
                {
                    var currentCredit = await _context.Credit
                        .FirstOrDefaultAsync(x => x.IdCredit == credit.IdCredit, stoppingToken);

                    //общее количество платежей, которые должны были уже быть выплачены на текущий момент
                    var allDaysPayment = (int) (DateTime.Now.Date - credit.DateCredit.Date.Date).TotalDays;
                    //количество уже выплаченных дней
                    var countDaysPayment = _context.Payment.Count(x => x.IdCredit == credit.IdCredit);
                    //количество необходимых выплат
                    var mustPayment = allDaysPayment - countDaysPayment;

                    //если выплата не требуется или депозит был взят сегодня
                    if (mustPayment == 0 || credit.DateCredit.Date == DateTime.Now.Date)
                    {
                        continue;
                    }

                    //находим счет для снятия средств
                    var currentBankAccount = await _context.BankAccount
                        .FirstOrDefaultAsync(x => x.IdAccount == credit.IdAccount, stoppingToken);

                    //расчет выплаты
                    var percentAmount = await CalcPercentAmount(credit);
                    //на сколько платежей хватит средств
                    var opportunityPaymentCount =
                        Convert.ToInt32(Math.Floor(currentBankAccount.Amount / percentAmount));

                    //если нет возможности оплатить ни одного срока
                    if (opportunityPaymentCount == 0)
                    {
                        //увеличение процентов
                        currentCredit.PercentCredit *= 2 * mustPayment;
                        
                        //сообщаем о нехватке средств
                        await _mediator.Send(
                            request: new BankAccountOperationCommand(idAccount: credit.IdAccount,
                                type: "Недостаточно средств (снятие по кредиту)", amount: 0),
                            cancellationToken: stoppingToken);
                        continue;
                    }

                    if (opportunityPaymentCount > 0 && opportunityPaymentCount < mustPayment)
                    {
                        //увеличение процентов
                        currentCredit.PercentCredit *= 2;

                        //сохраняем число возможных платежей для произведения снятия
                        mustPayment = opportunityPaymentCount;
                        
                        //сообщаем о нехватке средств на все выплаты
                        await _mediator.Send(
                            request: new BankAccountOperationCommand(idAccount: credit.IdAccount,
                                type: "Недостаточно средств (снятие по кредиту)", amount: 0),
                            cancellationToken: stoppingToken);

                        await _context.SaveChangesAsync(stoppingToken);
                    }

                    //наши необходимые снятия со счета
                    var paymentsData = new List<Payment>();
                    for (int i = 0; i < mustPayment; i++)
                    {
                        paymentsData.Add(new Payment()
                        {
                            IdCredit = credit.IdCredit,
                            AmountPayment = percentAmount,
                            DatePayment = DateTime.Now
                        });
                    }

                    await _context.Payment.AddRangeAsync(paymentsData, stoppingToken);
                    await _context.SaveChangesAsync(stoppingToken);

                    for (int i = 0; i < mustPayment; i++)
                    {
                        //снимаем средства
                        currentBankAccount.Amount -= percentAmount;
                        
                        //Фиксируем снятия в операциях
                        await _mediator.Send(new BankAccountOperationCommand(
                            credit.IdAccount, 
                            "Снятие по кредиту", 
                            percentAmount), stoppingToken);
                    }
                }

                await Task.Delay(millisecondsDelay: 86_400_000, cancellationToken: stoppingToken);
            }
        }
    }
}