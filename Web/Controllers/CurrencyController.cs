using System;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers
{
    /// <summary>
    ///   Контроллер для работы с валютой
    /// </summary>
    [AllowAnonymous]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyViewModelService _currencyViewModelService;

        public CurrencyController(ICurrencyViewModelService currencyViewModelService) =>
            _currencyViewModelService = currencyViewModelService;

        /// <summary>
        ///   Просмотре текущего курса валют
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 90, NoStore = true,
            VaryByHeader = "User-Agent")]
        public async Task<IActionResult> GetInfo() => View(await _currencyViewModelService.GetCurrencyRate());

        /// <summary>
        ///   Редактирование валюты доступно только при просмотре своего счёта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id) =>
            View(await _currencyViewModelService.GetBankAccountViewModel(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] EditBankAccountViewModel bankAccountViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _currencyViewModelService.ChangeAccountCurrency(id, bankAccountViewModel.TargetCurrencyId);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                return LocalRedirect("~/BankAccount/GetAccounts");
            }

            return View();
        }
    }
}