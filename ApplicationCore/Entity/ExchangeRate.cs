using System;
using System.Collections.Generic;

namespace ApplicationCore.Entity
{
    public partial class ExchangeRate
    {
        public DateTime DateRate { get; set; }
        public int IdCurrency { get; set; }
        public decimal RateSale { get; set; }
        public decimal RateBuy { get; set; }

        public virtual Currency IdCurrencyNavigation { get; set; }
    }
}
