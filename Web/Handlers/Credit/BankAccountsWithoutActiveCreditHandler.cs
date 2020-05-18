using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ApplicationCore.Entity;
using ApplicationCore.Interfaces;

using BankingSystem.ApplicationCore.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore.Internal;

using Web.Commands.Credit;

namespace Web.Handlers.Credit
{
  public class BankAccountsWithoutActiveCreditHandler : IRequestHandler<GetBankAccountsWithoutActiveCreditQuery,
                                                           List<BankAccount>>
  {
    private readonly IBankAccountRepository   _bankAccountRepository;
    private readonly ICreditRepository        _creditRepository;
    public           IAsyncRepository<Client> ClientRepository { get; set; }

    public BankAccountsWithoutActiveCreditHandler(IBankAccountRepository   bankAccountRepository,
                                                  ICreditRepository        creditRepository,
                                                  IAsyncRepository<Client> clientRepository)
    {
      _bankAccountRepository = bankAccountRepository;
      _creditRepository      = creditRepository;
      ClientRepository       = clientRepository;
    }

    public async Task<List<BankAccount>> Handle(GetBankAccountsWithoutActiveCreditQuery request,
                                                CancellationToken                       token)
    {
      var accounts = new List<BankAccount>(_bankAccountRepository
                                          .Accounts
                                          .Where(p => p.IdClient       == request.IdClient
                                                      && p.AccountType == "кредитный"
                                                      && p.DateClose   == null));

      //тестани а вдруг
      // var client =await ClientRepository.GetById(request.IdClient);
      // client.BankAccounts.Where(account => account.AccountType == "кредитный");
      // foreach(var account in client.BankAccounts.Where(account => account.Credits.Any())) account.Credits.Where(credit => credit.Status);

      //счета, на которых нет активного кредита
      var finishCredits = new List<BankAccount>();

      //На код ниже не смотрите, это необходимый синтаксический костыль
      foreach (var account in accounts)
      {
        var creditsAccount = _creditRepository.Credits.Where(p => p.IdAccount == account.IdAccount && p.Status);

        if (!creditsAccount.Any()) finishCredits.Add(account);
      }

      return finishCredits;
    }
  }
}
