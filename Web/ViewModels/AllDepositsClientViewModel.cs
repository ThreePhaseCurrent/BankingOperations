using System.Collections;
using System.Collections.Generic;
using ApplicationCore.Entity;

namespace Web.ViewModels
{
    public class AllDepositsClientViewModel
    {
        public int IdClient { get; set; }
        public IEnumerable<Deposit> Deposits { get; set; } 
    }
}