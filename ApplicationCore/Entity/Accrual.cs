using System;
using System.Collections.Generic;

namespace ApplicationCore.Entity
{
    public partial class Accrual
    {
        public int IdAccrual { get; set; }
        public DateTime AccrualDate { get; set; }
        public decimal AccrualAmount { get; set; }
        public int IdDeposit { get; set; }

        public virtual Deposit IdDepositNavigation { get; set; }
    }
}
