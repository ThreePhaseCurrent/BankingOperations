using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

using BankingSystem.ApplicationCore.Interfaces;

using Infrastructure.Extensions;
using Infrastructure.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Web.ViewModels;

namespace Web.Controllers
{
  [Authorize(Roles = AuthorizationConstants.Roles.CLIENT + ", " + AuthorizationConstants.Roles.ADMINISTRATOR)]
  public class BankAccountController : Controller
  {
    private readonly IBankAccountRepository _accountRepository;
    private readonly IAsyncRepository<Client> _clientRepository;

    public BankAccountController(IBankAccountRepository accountRepository,
                                 IAsyncRepository<Client> clientRepository)
    {
      _accountRepository = accountRepository;
      _clientRepository = clientRepository;
    }

    /// <summary>
    ///   Добавление нового счета существующего клиента.
    ///   Возвращает представление и модель с физическими и юридическими лицами, для вывода частичной информации.
    /// </summary>
    /// <param name="idClient"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> CreateClientAccountForm(int? idClient)
    {
      Client client;
      if (idClient == default)
      {
        var spec = new UserByLoginSpecification(User.Identity.Name);
        client = await _clientRepository.SingleBy(spec);
      }
      else
      {
        client = await _clientRepository.GetById(idClient);
      }

      return View(new CreateClientAccountViewModel
      {
        PhysicalPerson = client.PhysicalPerson,
        LegalPerson = client.LegalPerson,
        ReturnUrl = "/"
      });
    }

    /// <summary>
    ///   Создание счета на основе заполненной формы
    /// </summary>
    /// <param name="createClientAccountViewModel"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateClientAccountForm(CreateClientAccountViewModel createClientAccountViewModel)
    {
      if (ModelState.IsValid)
      {
        //сохранение счета
        await _accountRepository.SaveAccount(createClientAccountViewModel.Account);
        return RedirectToAction("GetAccounts", "BankAccount",
                                new { idClient = createClientAccountViewModel.Account.IdClient });
      }

      return View();
    }

    /// <summary>
    ///   Отображение всех счетов клиента
    ///   Точки входа:
    ///
    ///   1)_Layout - редирект на логин так поскольку аттр на контроллере висит
    ///   2)EditCurrency - вернутся назад
    /// </summary>
    /// <returns></returns>
    ///
    [HttpGet]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 90, NoStore = true)]
    public async Task<IActionResult> GetAccounts(int idClient = default)
    {
      Client client;
      if (idClient == default)
        client =
                await _clientRepository.SingleBy(new UserByLoginSpecification(User.Identity.Name));
      else if (idClient is var _)
        client = await _clientRepository.GetById(idClient);

      var accountViewModel = new AccountViewModel
      {
        IdClient = client.IdClient,
        BankAccounts = client.BankAccount,
        ModalError = HttpContext.Session.Get<PopupViewModel>("AccountPopup")
      };

      HttpContext.Session.Remove("AccountPopup");

      return View(accountViewModel);
    }

    /// <summary>
    ///   Закрытие счета клиента
    /// </summary>
    /// <param name="idAccount"></param>
    /// <returns></returns>
    public async Task<IActionResult> BankAccountClose(int idAccount)
    {
      var account = await _accountRepository.Accounts.FirstOrDefaultAsync(x => x.IdAccount == idAccount);

      var result = await _accountRepository.CloseAccount(idAccount);

      //если счет не закрылся, формируем ошибку
      if(!result)
        HttpContext.Session.Set("AccountPopup",
                                new PopupViewModel
                                {
                                  Title = "Внимание!",
                                  Message =
                                    "Счет имеет незавершенный депозит или кредит. Закрытие счета невозможно."
                                });

      return RedirectToAction("GetAccounts", new
      {
        account.IdClient
      });
    }

    //Перед тем, как удалить счет, нужно его закрыть методом BankAccountClose
    /// <summary>
    ///   Удаление счета клиента
    /// </summary>
    /// <param name="idAccount"></param>
    /// <returns></returns>
    public async Task<IActionResult> BankAccountDelete(int idAccount)
    {
      var account = await _accountRepository.Accounts.FirstOrDefaultAsync(x => x.IdAccount == idAccount);

      var result = await _accountRepository.DeleteAccount(idAccount);

      //если счет не закрылся, формируем ошибку
      if(!result)
        HttpContext.Session.Set("AccountPopup",
                                new PopupViewModel
                                {
                                  Title = "Внимание!",
                                  Message = "Счет имеет остаточный баланс. Невозможно удалить счёт."
                                });

      return RedirectToAction("GetAccounts", new
      {
        account.IdClient
      });
    }
  }
}