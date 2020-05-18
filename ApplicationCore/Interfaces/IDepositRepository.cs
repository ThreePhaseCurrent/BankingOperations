using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entity;

namespace ApplicationCore.Interfaces
{
    public interface IDepositRepository
    {
        IQueryable<Deposit> Deposits { get; }

        Task AddDeposit(Deposit deposit);
        Task CloseDeposit(Deposit deposit);
        
        /// <summary>
        /// Начисление процентов
        /// </summary>
        /// <param name="idDeposit"></param>
        /// <returns></returns>
        Task<bool> PercentAccrual(int idDeposit);

        /// <summary>
        /// Закрытие завершенных депозитов
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Deposit>> CloseFinishedDeposits();
    }
}