using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

namespace Web.Commands.Operation
{
    /// <summary>
    ///   Команда для фиксации операций с аккаунтом
    /// </summary>
    public class BankAccountOperationCommand : IRequest
    {
        /// <summary>
        ///   Id - аккаунта
        /// </summary>
        public int IdAccount { get; }

        /// <summary>
        ///   Тип операции
        /// </summary>
        public string Type { get; }

        /// <summary>
        ///   Сумма на которую была произведена операция
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        ///   Инициализирует все данные про операцию
        /// </summary>
        /// <param name="idAccount">Id - банковского аккаунта</param>
        /// <param name="type">Тип операции</param>
        /// <param name="amount">Сумма на которую была пшроизведена операция</param>
        public BankAccountOperationCommand(int idAccount, string type, decimal amount)
        {
            IdAccount = idAccount;
            Type = type;
            Amount = amount;
        }
    }
}
