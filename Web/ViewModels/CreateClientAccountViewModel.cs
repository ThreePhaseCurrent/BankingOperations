using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApplicationCore.Entity;

namespace Web.ViewModels
{
  public class CreateClientAccountViewModel
  {
    public PhysicalPerson PhysicalPerson { get; set; }
    public LegalPerson LegalPerson { get; set; }
    public ApplicationCore.Entity.BankAccount Account { get; set; }
    public string ReturnUrl { get; set; } = "/";
  }
}
