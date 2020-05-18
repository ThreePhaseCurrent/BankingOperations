using System.Collections.Generic;
using ApplicationCore.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.ViewModels
{
    public class EditBankAccountViewModel
    {
        public int    CurrentCurrencyId   { get; set; }
        public string CurrentCurrencyName { get; set; }

        public int TargetCurrencyId { get; set; }

        public decimal    CurrentAmount      { get; set; }
        public IEnumerable<Currency> CurrencyList { get; set; }
        public int IdAccount { get; set; }
    }
}