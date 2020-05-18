using System.Collections.Generic;
using System.Threading.Tasks;
using Web.ViewModels;

namespace ApplicationCore.Interfaces
{
    /// <summary>
    ///   Сервис реализующий основную логику связаную с банковским аккаунтом и валютой
    /// </summary>
    public interface ICurrencyViewModelService
    {
        Task<List<CurrencyViewModel>>  GetCurrencyRate();
        Task<EditBankAccountViewModel> GetBankAccountViewModel(int id);

        Task ChangeAccountCurrency(int accountId, int targetId);
    }
}