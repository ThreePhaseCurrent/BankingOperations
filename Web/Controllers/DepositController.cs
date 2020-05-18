using System;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using BankingSystem.ApplicationCore.Interfaces;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Commands;
using Web.Commands.Deposit;
using Web.ViewModels;
using System.Web;

namespace Web.Controllers
{
    [Authorize(Roles = AuthorizationConstants.Roles.CLIENT + ", " + AuthorizationConstants.Roles.ADMINISTRATOR)]
    public class DepositController : Controller
    {
        private readonly IMediator _mediator;
        private IDepositRepository _depositRepository;
        
        public DepositController(IMediator mediator, IDepositRepository depositRepository)
        {
            _mediator = mediator;
            _depositRepository = depositRepository;
        }

        public async Task<IActionResult> AllDeposits(int? idClient)
        {
            if (idClient == default)
            {
                idClient = await _mediator.Send(new GetUserByIdQuery(User.Identity.Name));
            }
            
            var deposits = _depositRepository.Deposits
                .Where(x => x.IdAccountNavigation.IdClient == idClient && x.Status);

            return View(model: new AllDepositsClientViewModel() {IdClient = (int) idClient, Deposits = deposits});
        }

        [HttpGet]
        public async Task<IActionResult> MakeDeposit(int idClient)
        {
            var freeDeposits = await _mediator.Send(new GetBankAccountsWithoutActiveDepositQuery(idClient));

            if (freeDeposits.Count == 0)
            {
                return RedirectToAction("NonAccounts", new Client(){IdClient = idClient});
            }

            return View(new MakeDepositViewModel(){IdClient = idClient, BankAccounts = freeDeposits});
        }

        [HttpPost]
        public async Task<IActionResult> MakeDeposit(MakeDepositViewModel makeDepositViewModel)
        {
            if (ModelState.IsValid)
            {
                var deposit = makeDepositViewModel.Deposit;

                var applyDeposit = await _mediator.Send(new ApplyDepositCommand(deposit));

                if (applyDeposit == false)
                {
                    ModelState.AddModelError("NotEnoughMoney", "Недостаточно средств для оформления депозита.");

                    makeDepositViewModel.BankAccounts =
                        await _mediator.Send(
                            new GetBankAccountsWithoutActiveDepositQuery(makeDepositViewModel.IdClient));
                    
                    return View(makeDepositViewModel);
                }

                return RedirectToAction("AllDeposits", new {idClient = makeDepositViewModel.IdClient});
            }

            return View();
        }

        public async Task<IActionResult> NonAccounts(Client client)
        {
            return View(client);
        }

        public async Task<IActionResult> Test()
        {
            var days1 = Convert.ToDateTime("26.04.2020");
            var days2 = DateTime.Now.Date;
            var days = (days1.AddDays(1) - days2).TotalDays;

            return RedirectToAction("AllDeposits");
        }
    }
}