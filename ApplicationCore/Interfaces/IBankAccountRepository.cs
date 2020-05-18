using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using ApplicationCore.Entity;

namespace ApplicationCore.Interfaces
{
    public interface IBankAccountRepository
    {
        IQueryable<BankAccount> Accounts { get; }

        Task SaveAccount(BankAccount account);
        Task<bool> DeleteAccount(int idAccount);
        Task<bool> CloseAccount(int idAccount);
    }
}
