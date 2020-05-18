using System;
using System.Collections.Generic;

namespace ApplicationCore.Entity
{
    public partial class Deposit
    {
        public Deposit()
        {
            Accrual = new HashSet<Accrual>();
        }

        public int IdDeposit { get; set; }
        public int IdAccount { get; set; }
        public decimal PercentDeposit { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateDeposit { get; set; }
        public DateTime DateDepositFinish { get; set; }
        public bool Status { get; set; }

        public virtual BankAccount IdAccountNavigation { get; set; }
        public virtual ICollection<Accrual> Accrual { get; set; }
    }
}
