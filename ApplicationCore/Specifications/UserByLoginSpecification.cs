using System;
using System.Collections.Generic;
using System.Text;

using ApplicationCore.Entity;

namespace ApplicationCore.Specifications
{
  public class UserByLoginSpecification : BaseSpecification<Client>
  {
    public UserByLoginSpecification(string login) : base(client => client.Login.Equals(login)) { }
  }

  public class BankAccountOperationSpecification : BaseSpecification<Operation>
  {
    public BankAccountOperationSpecification(int id) : base(operation => operation.IdAccount == id) { }
  }
}
