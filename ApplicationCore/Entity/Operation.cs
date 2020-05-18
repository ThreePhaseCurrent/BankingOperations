using System;
using System.Collections.Generic;

namespace ApplicationCore.Entity
{
    public partial class Operation
    {
        public DateTime OperationTime { get; set; }
        public int IdAccount { get; set; }
        public string TypeOperation { get; set; }
        public decimal Amount { get; set; }

        public virtual BankAccount IdAccountNavigation { get; set; }
    }
}
