using System;
using System.Collections.Generic;

namespace ApplicationCore.Entity
{
    public partial class Credit
    {
        public Credit()
        {
            Payment = new HashSet<Payment>();
        }

        public int IdCredit { get; set; }
        public int IdAccount { get; set; }
        public decimal PercentCredit { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCredit { get; set; }
        public DateTime DateCreditFinish { get; set; }
        public bool Status { get; set; }

        public virtual BankAccount IdAccountNavigation { get; set; }
        public virtual ICollection<Payment> Payment { get; set; }
    }
}
