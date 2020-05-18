using System;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using BankingSystem.ApplicationCore.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Infrastructure.Data
{
    public class BankAccountEfRepository : IBankAccountRepository
    {
        private readonly BankOperationsContext _context;

        public BankAccountEfRepository(BankOperationsContext context) => _context = context;

        public IQueryable<BankAccount> Accounts => _context.BankAccount;

        /// <summary>
        ///   Создание счета или сохранение изменений
        /// </summary>
        /// <param name="account"></param>
        public async Task SaveAccount(BankAccount account)
        {
            if (account.IdAccount == 0) await _context.BankAccount.AddAsync(entity: account);
            else
            {
                var bankAccount = await _context.BankAccount.FirstAsync(predicate: s => s.IdAccount == account.IdAccount);

                if (bankAccount != null)
                {
                    bankAccount.AccountType = account.AccountType;
                    bankAccount.Amount = account.Amount;
                    bankAccount.DateClose = account.DateClose;
                    bankAccount.IdCurrency = account.IdCurrency;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAccount(int idAccount)
        {
            var bankAccount = await _context.BankAccount.FirstAsync(predicate: account => account.IdAccount == idAccount);

            if (bankAccount?.DateClose != null
               && bankAccount.Amount == 0)
            {
                _context.BankAccount.Remove(entity: bankAccount);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> CloseAccount(int idAccount)
        {
            var bankAccount = await _context.BankAccount.FirstAsync(predicate: account => account.IdAccount == idAccount);

            var activeCredit = await _context.Credit.FirstOrDefaultAsync(predicate: x => x.IdAccount == idAccount && x.Status);
            var activeDeposit = await _context.Deposit.FirstOrDefaultAsync(predicate: x => x.IdAccount == idAccount && x.Status);

            if (bankAccount != null
               && activeCredit == null
               && activeDeposit == null
               && bankAccount.DateClose == null)
            {
                bankAccount.DateClose = DateTime.Now;

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }


}
