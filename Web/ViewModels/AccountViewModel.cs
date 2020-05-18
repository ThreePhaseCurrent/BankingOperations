using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApplicationCore.Entity;

namespace Web.ViewModels
{
    public class AccountViewModel
    {
        public int? IdClient { get; set; }
        public IEnumerable<BankAccount> BankAccounts { get; set; }
        public PopupViewModel ModalError { get; set; }
    }
}
