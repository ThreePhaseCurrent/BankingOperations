using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApplicationCore.Entity;

using MediatR;

namespace Web.Commands.Credit
{
  public class GetBankAccountsWithoutActiveCreditQuery : IRequest<List<BankAccount>>
  {
    /// <summary>
    ///   Идентификатор пользователя
    /// </summary>
    public int IdClient { get; }

    /// <summary>
    ///   Получение кредитных счетов без активных кредитов
    /// </summary>
    /// <param name="idClient">Идентификатор пользователя</param>
    public GetBankAccountsWithoutActiveCreditQuery(int idClient) => IdClient = idClient;
  }
}
