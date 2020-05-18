using System.Collections;
using System.Collections.Generic;
using ApplicationCore.Entity;

namespace Web.ViewModels
{
    public class MakeDepositViewModel
    {
        public int IdClient { get; set; }
        public IEnumerable<BankAccount> BankAccounts { get; set; }
        public Deposit Deposit { get; set; }
    }
}