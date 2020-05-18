using System;
using System.Collections.Generic;

namespace ApplicationCore.Entity
{
    public partial class Currency
    {
        public Currency()
        {
            BankAccount = new HashSet<BankAccount>();
            ExchangeRate = new HashSet<ExchangeRate>();
        }

        public int IdCurrency { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        public virtual ICollection<BankAccount> BankAccount { get; set; }
        public virtual ICollection<ExchangeRate> ExchangeRate { get; set; }
    }
}
