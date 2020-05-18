using System;
using System.Collections.Generic;

namespace ApplicationCore.Entity
{
    public partial class BankAccount
    {
        public BankAccount()
        {
            Credit = new HashSet<Credit>();
            Deposit = new HashSet<Deposit>();
            Operation = new HashSet<Operation>();
        }

        public int IdAccount { get; set; }
        public int IdClient { get; set; }
        public string AccountType { get; set; }
        public int IdCurrency { get; set; }
        public DateTime DateOpen { get; set; }
        public DateTime? DateClose { get; set; }
        public decimal Amount { get; set; }

        public virtual Client IdClientNavigation { get; set; }
        public virtual Currency IdCurrencyNavigation { get; set; }
        public virtual ICollection<Credit> Credit { get; set; }
        public virtual ICollection<Deposit> Deposit { get; set; }
        public virtual ICollection<Operation> Operation { get; set; }
        
        /// <summary>
        /// Изменение валюту аккаунта
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="rate"></param>
        public void ChangeCurrency(Currency currency, decimal rate)
        {
            IdCurrency = currency.IdCurrency;
            this.IdCurrencyNavigation = currency;
            Amount *= rate;
        }
    }
}
