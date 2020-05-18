using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DepositRepository : IDepositRepository
    {
        private readonly BankOperationsContext _context;
        
        public DepositRepository(BankOperationsContext context)
        {
            _context = context;
        }

        public IQueryable<Deposit> Deposits => _context.Deposit;
        
        public async Task AddDeposit(Deposit deposit)
        {
            await _context.Deposit.AddAsync(deposit);
            await _context.SaveChangesAsync();
        }
        
        public async Task<bool> PercentAccrual(int idDeposit)
        {
            var deposit = await _context.Deposit.FirstOrDefaultAsync(x => x.IdDeposit == idDeposit);

            if (deposit == null)
            {
                return false;
            }

            //общее количество платежей, которые должны были уже быть выплачены на текущий момент
            var allDaysPayment = (int)(DateTime.Now.Date - deposit.DateDeposit.Date.Date).TotalDays;
            //количество уже выплаченных дней
            var countDaysPayment = _context.Accrual.Count(x => x.IdDeposit == deposit.IdDeposit);
            //количество необходимых выплат
            var mustPayment = allDaysPayment - countDaysPayment;

            //если выплата не требуется или депозит был взят сегодня
            if (mustPayment == 0 || deposit.DateDeposit.Date == DateTime.Now.Date)
            {
                return true;
            }
            
            //расчет выплаты
            var percentAmount = await CalcPercentAmount(deposit);
            deposit.Amount += percentAmount * mustPayment;

            var accrualsData = new List<Accrual>();
            for (var i = 0; i < mustPayment; i++)
            {
                accrualsData.Add(new Accrual()
                {
                    IdDeposit = deposit.IdDeposit,
                    AccrualDate = DateTime.Now,
                    AccrualAmount = percentAmount
                });
            }

            await _context.Accrual.AddRangeAsync(accrualsData);
            await _context.SaveChangesAsync(); 
            
            return true;
        }

        //расчет суммы процента депозита
        private async Task<decimal> CalcPercentAmount(Deposit deposit)
        {
            Operation startDeposit = null;
            try
            {
                startDeposit = await _context.Operation
                    .OrderByDescending(x => x.OperationTime)
                    .FirstAsync(x => x.TypeOperation == "Передача средств на депозит");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            return startDeposit.Amount * deposit.PercentDeposit / 100;
        }

        public async Task<IEnumerable<Deposit>> CloseFinishedDeposits()
        {
            //все активные депозиты
            var deposits = _context.Deposit.Where(x => x.Status).ToList();
            var closedDeposits = new List<Deposit>();

            foreach (var deposit in deposits)
            {
                //количество обязательных выплат
                var mustPayments = (int)(deposit.DateDepositFinish - deposit.DateDeposit.Date).TotalDays;
                //количество совершенных выплат
                var countPayments = await _context.Accrual
                    .CountAsync(x => x.IdDeposit == deposit.IdDeposit);
                
                if (mustPayments <= countPayments)
                {
                    //закрытие депозита
                    var finishedDeposit = await _context.Deposit
                        .FirstOrDefaultAsync(x => x.IdDeposit == deposit.IdDeposit);
                    finishedDeposit.Status = false;
                    
                    //в список закрытых депозитов
                    closedDeposits.Add(finishedDeposit);

                    await _context.SaveChangesAsync();
                }
            }

            return closedDeposits;
        }

        public async Task CloseDeposit(Deposit deposit)
        {
            throw new System.NotImplementedException();
        }
    }
}