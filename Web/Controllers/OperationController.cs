using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using BankingSystem.ApplicationCore.Interfaces;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Commands.Operation;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize(Roles = AuthorizationConstants.Roles.CLIENT + ", " + AuthorizationConstants.Roles.ADMINISTRATOR)]
    public class OperationController : Controller
    {
        private readonly IAsyncRepository<BankAccount> _bankAccountRepository;
        private readonly ILogger<OperationController> _logger;
        private readonly IMediator _mediator;

        public OperationController(IMediator mediator,
            IAsyncRepository<BankAccount> bankAccountRepository,
            ILogger<OperationController> logger)
        {
            _mediator = mediator;
            _bankAccountRepository = bankAccountRepository;
            _logger = logger;
        }

        /// <summary>
        ///   Просмотр уже имеющихся операций по аккаунту, возмодность выбрать их за определённый период, выполнить новые операции.
        ///   Гарантирует что пользователь с балансом 0 - не попадёт на страничку с переводом денег.
        ///   Гарантирует что пользователь с кридетами,депозитами или с балансом 0 не попадёт на страничку с конвертацией валюты
        ///   своего текущего аккаунта.
        /// </summary>
        /// <param name="id">Просматриваемый в текущий момент банковский счёт</param>
        /// <param name="formModel"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int id, AccountOperationViewModel formModel)
        {
            _logger.LogInformation($"Get index operation with id: {id}, view model : {formModel}");
            var viewModel = await _mediator.Send(new GetAccountOperationQuery(id,
                formModel.StartPeriod,
                formModel.EndPeriod));
            ViewBag.CanTransfer = await _mediator.Send(new TransferValidationCommand(id));
            ViewBag.CorrectAmount = viewModel.Amount > 0;
            
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Transfer(int id)
        {
            _logger.Log(LogLevel.Information, $"Transfer get from account with id: {id}");
            var transferViewModel = new TransferViewModel {IdFrom = id};
            return View(transferViewModel);
        }

        // !TODO: Доделать проверку на перевод денег только с карты на карту с одинаковой валютой
        //Доделать проверку на не превишение суммы которая есть на акк, проверку на существование акк на который скидываем деньги
        [HttpPost]
        public async Task<IActionResult> Transfer([FromForm] TransferViewModel transferViewModel)
        {
            _logger.LogInformation($"Transfer post start processing\nview model: {transferViewModel}");

            var account = await _bankAccountRepository.GetById(transferViewModel.IdFrom);
            var accountReceiver = await _bankAccountRepository.GetById(transferViewModel.IdTo);

            if (TryValidateModel(ModelState))
            {
                _logger.LogInformation($"Check correct transfer amount - {account.Amount < transferViewModel.Amount}");
                if (account.Amount < transferViewModel.Amount)
                    ModelState.AddModelError("Amount", "Недостаточно средств!");
                _logger.LogInformation(
                    $"Check equals currency in account sender and accout receiver: {accountReceiver.IdCurrency.Equals(account.IdCurrency)}");

                if (!accountReceiver.IdCurrency.Equals(account.IdCurrency))
                    ModelState.AddModelError("Amount",
                        "Не возможно перевести деньги на карту в другой валюте!");
            }

            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new TransferAmountCommand(transferViewModel.IdFrom,
                    transferViewModel.IdTo,
                    transferViewModel.Amount));

                return RedirectToAction("GetAccounts", "BankAccount", account.IdAccount);
            }

            return View("Transfer", transferViewModel);
        }

        /// <summary>
        ///   Проверка существует ли карточка на которую человек хочет перевести деньги
        /// </summary>
        /// <param name="IdTo"></param>
        /// <returns></returns>
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> AccountExist([FromQuery] int IdTo)
        {
            var exist = await _mediator.Send(new CheckAccountExistQuery(IdTo));
            _logger.LogInformation($"Check exist account {IdTo}\nAccount is exist {exist} ");

            return Json(exist);
        }

        public async Task<IActionResult> ActivateAccount(int id)
        {
            _logger.LogInformation($"Activate account {id}");

            var account = await _bankAccountRepository.GetById(id);

            if (account.DateClose != null)
            {
                account.DateClose = null;
                await _bankAccountRepository.UpdateAsync(account);
            }

            _logger.LogInformation($"Account close {account.DateClose}");

            return RedirectToAction("GetAccounts", "BankAccount", new {idClient = account.IdClient});
        }
    }
}