using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;
using BankingSystem.ApplicationCore.Interfaces;
using MediatR;
using Web.Commands.Deposit;

namespace Web.Handlers.Deposit
{
    public class
        BankAccountsWithoutActiveDepositHandle : IRequestHandler<GetBankAccountsWithoutActiveDepositQuery,
            List<BankAccount>>
    {
        private IDepositRepository _depositRepository;
        private IBankAccountRepository _accountRepository;

        public BankAccountsWithoutActiveDepositHandle(
            IDepositRepository depositRepository,
            IBankAccountRepository bankAccountRepository)
        {
            _accountRepository = bankAccountRepository;
            _depositRepository = depositRepository;
        }

        public async Task<List<BankAccount>> Handle(GetBankAccountsWithoutActiveDepositQuery query
            , CancellationToken token)
        {
            var findAccounts = new List<BankAccount>();

            var depositAccounts = _accountRepository.Accounts
                .Where(x => x.IdClient == query.IdClient && x.AccountType == "депозитный"
                && x.DateClose == null
                && x.Amount >= 1000);

            foreach (var account in depositAccounts)
            {
                var activeDeposits = _depositRepository.Deposits
                    .Where(p => p.IdAccount == account.IdAccount && p.Status);

                if (!activeDeposits.Any()) findAccounts.Add(account);
            }

            return findAccounts;
        }
    }
}