using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApplicationCore.Entity;
using ApplicationCore.Interfaces;

using Infrastructure.Identity;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Web.Commands;
using Web.Commands.Credit;
using Web.ViewModels;

namespace Web.Controllers
{
  /// <summary>
  ///   Оформление, жизненный цикл кредитов
  /// </summary>
  [Authorize(Roles = AuthorizationConstants.Roles.CLIENT + ", " + AuthorizationConstants.Roles.ADMINISTRATOR)]
  public class CreditController : Controller
  {
    private readonly ICreditRepository _creditRepository;
    private readonly IMediator _mediator;

    public CreditController(ICreditRepository creditRepository, IMediator mediator)
    {
      _creditRepository = creditRepository;
      _mediator = mediator;
    }

    /// <summary>
    ///   Оформление кредита пользователем или менеджером
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> TakeCreditForm(int idClient)
    {
      //получение кредитных счетов, у которых нет активных кредитов
      var finishCredits = await _mediator.Send(new GetBankAccountsWithoutActiveCreditQuery(idClient));

      if (finishCredits.Count == 0)
      {
        return RedirectToAction("NonAccounts", new Client { IdClient = idClient });
      }

      return View(new TakeCreditViewModel { IdClient = idClient, BankAccounts = finishCredits });
    }

    [HttpPost]
    public async Task<IActionResult> TakeCreditForm(TakeCreditViewModel takeCreditViewModel)
    {
      if (ModelState.IsValid)
      {
        //оформление кредита
        var applyCredit = await _mediator.Send(new ApplyCreditCommand(takeCreditViewModel.Credit));
        return RedirectToAction("AllCredits", new { idClient = takeCreditViewModel.IdClient });
      }

      return View();
    }

    /// <summary>
    ///   Получение всех кредитов пользователя
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> AllCredits(int? idClient)
    {
      if (idClient == null)
        idClient = await _mediator.Send(new GetUserByIdQuery(User.Identity.Name));

      var data =
              _creditRepository.Credits
                               .Where(account => account.IdAccountNavigation.IdClient == idClient
                                                 && account.Status);

      return View(new AllCreditClientViewModel { Credits = data, IdClient = idClient });
    }

    public IActionResult NonAccounts(Client client) => View(client);
  }
}
