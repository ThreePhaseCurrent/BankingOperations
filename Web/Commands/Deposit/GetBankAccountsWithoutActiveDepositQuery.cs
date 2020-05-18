using System.Collections.Generic;
using ApplicationCore.Entity;
using MediatR;

namespace Web.Commands.Deposit
{
    public class GetBankAccountsWithoutActiveDepositQuery : IRequest<List<BankAccount>>
    {
        public int IdClient { get;}
        
        /// <summary>
        /// Получение счетов без активных депозитов
        /// </summary>
        /// <param name="idClient"></param>
        public GetBankAccountsWithoutActiveDepositQuery(int idClient)
        {
            IdClient = idClient;
        }
    }
}