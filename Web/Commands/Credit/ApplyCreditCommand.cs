using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

namespace Web.Commands.Credit
{
  public class ApplyCreditCommand : IRequest<bool>
  {
    /// <summary>
    ///   Кредит, который необходимо оформить
    /// </summary>
    public ApplicationCore.Entity.Credit Credit { get; set; }

    /// <summary>
    ///   Команда оформления кредита
    /// </summary>
    /// <param name="credit">Кредит, который необходимо оформить</param>
    public ApplyCreditCommand(ApplicationCore.Entity.Credit credit) => Credit = credit;
  }
}
