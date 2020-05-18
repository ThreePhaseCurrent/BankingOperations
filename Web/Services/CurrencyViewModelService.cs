using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using BankingSystem.ApplicationCore.Interfaces;

using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Web.Commands.Operation;
using Web.Services.HttpClient;
using Web.ViewModels;

namespace Web.Services
{
    public class CurrencyViewModelService : ICurrencyViewModelService
  {
    private readonly IAsyncRepository<BankAccount>     _bankAccountRepository;
    private readonly CurrencyExchangeService           _currencyExchangeService;
    private readonly IAsyncRepository<Currency>        _currencyRepository;
    private readonly ILogger<CurrencyViewModelService> _logger;
    private readonly IMediator                         _mediator;

    public CurrencyViewModelService(IAsyncRepository<BankAccount>     bankAccountRepository,
                                    IAsyncRepository<Currency>        currencyRepository,
                                    ILogger<CurrencyViewModelService> logger, IMediator mediator,
                                    CurrencyExchangeService           currencyExchangeService)
    {
      _bankAccountRepository   = bankAccountRepository;
      _currencyRepository      = currencyRepository;
      _logger                  = logger;
      _mediator                = mediator;
      _currencyExchangeService = currencyExchangeService;
    }

    /// <summary>
    ///   Получение курса валют
    /// </summary>
    /// <returns></returns>
    public async Task<List<CurrencyViewModel>> GetCurrencyRate()
    {
      _logger.LogInformation($"{nameof(GetCurrencyRate)} called");

      var currencies = await _currencyRepository.GetAll();

      var list = currencies.Select(currency => new {currency, lastUpdate = currency.ExchangeRate.Last()})
                           .Select(it => new CurrencyViewModel
                            {
                                    Id       = it.currency.IdCurrency,
                                    Name     = it.currency.Name,
                                    BuyRate  = it.lastUpdate.RateBuy,
                                    SaleRate = it.lastUpdate.RateSale
                            }).ToList();

      _logger.LogInformation($"Was returned {list.Count} currency view models.");

      return list;
    }

    /// <summary>
    ///   Отображаем Edit
    /// </summary>
    /// <param name="id">Аккаунт для которого формируем VM</param>
    /// <returns></returns>
    public async Task<EditBankAccountViewModel> GetBankAccountViewModel(int id)
    {
      //Получаем Акк
      var account    = await _bankAccountRepository.GetById(id);
      var currencies = await _currencyRepository.GetAll();

      var idCurrency = account.IdCurrency;
      var find       = currencies.Find(currency => currency.IdCurrency.Equals(idCurrency));
      currencies.Remove(find);

      var viewModel = new EditBankAccountViewModel
      {
              //Список с валютами
              CurrencyList  = currencies,
              CurrentCurrencyId   = account.IdCurrency,
              CurrentCurrencyName = account.IdCurrencyNavigation.ShortName,
              CurrentAmount       = account.Amount,
              IdAccount = account.IdAccount,
      };
      return viewModel;
    }

    /// <summary>
    ///   Изменение текущей валюты банковского аккаунта
    /// </summary>
    /// <param name="accountId">Текущий аккаунт</param>
    /// <param name="targetId">Выбраная валюта</param>
    /// <returns></returns>
    public async Task ChangeAccountCurrency(int accountId, int targetId)
    {
      //Получаем текущий акк и желаемую валюту
      var account  = await _bankAccountRepository.GetById(accountId);
      var currency = await _currencyRepository.GetById(targetId);

      //Смотри коеф в интернете
      var rate = await _currencyExchangeService.FetchRate(account.IdCurrencyNavigation.ShortName, currency.ShortName);

      //Меняем и сохраняем значение
      account.ChangeCurrency(currency, rate);
      await _bankAccountRepository.UpdateAsync(account);

      await _mediator.Send(new BankAccountOperationCommand(accountId, "Смена валюты счёта", amount: 0m));
    }
  }
}