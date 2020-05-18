using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApplicationCore.Entity;

namespace Web.ViewModels
{
  /// <summary>
  ///   Для получения всех кредитов пользователя
  /// </summary>
  public class AllCreditClientViewModel
  {
    public int? IdClient { get; set; }
    public IEnumerable<Credit> Credits { get; set; }
  }
}
