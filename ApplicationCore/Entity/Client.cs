using System;
using System.Collections.Generic;

namespace ApplicationCore.Entity
{
    public partial class Client
    {
        public Client()
        {
            BankAccount = new HashSet<BankAccount>();
        }

        public int IdClient { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string TelNumber { get; set; }

        public virtual LegalPerson LegalPerson { get; set; }
        public virtual PhysicalPerson PhysicalPerson { get; set; }
        public virtual ICollection<BankAccount> BankAccount { get; set; }
    }
}
