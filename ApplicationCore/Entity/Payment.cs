using System;
using System.Collections.Generic;

namespace ApplicationCore.Entity
{
    public partial class Payment
    {
        public int IdPayment { get; set; }
        public DateTime DatePayment { get; set; }
        public decimal AmountPayment { get; set; }
        public int IdCredit { get; set; }

        public virtual Credit IdCreditNavigation { get; set; }
    }
}
