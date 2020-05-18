using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Specifications;
using BankingSystem.ApplicationCore.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Commands.Operation;
using Web.ViewModels;

namespace Web.Handlers.Operation
{
    /// <summary>
  ///   Обработчик для запроса по операциям по акк
  /// </summary>
  public class AccountOperationHandler : IRequestHandler<GetAccountOperationQuery, AccountOperationViewModel>
  {
    private readonly IAsyncRepository<BankAccount>    _accountRepository;
    private readonly ILogger<AccountOperationHandler> _logger;

    private readonly IAsyncRepository<ApplicationCore.Entity.Operation> _operationRepository;

    public AccountOperationHandler(IAsyncRepository<BankAccount>                      accountRepository,
                                   IAsyncRepository<ApplicationCore.Entity.Operation> operationRepository,
                                   ILogger<AccountOperationHandler>                   logger)
    {
      _accountRepository   = accountRepository;
      _operationRepository = operationRepository;
      _logger              = logger;
    }

    public async Task<AccountOperationViewModel> Handle(GetAccountOperationQuery request,
                                                        CancellationToken        cancellationToken)
    {
      _logger.Log(LogLevel.Information, "AcountOperationionHandler call handler");
      _logger.Log(LogLevel.Information, $"Get account by request id: {request.Id}");

      var account = await _accountRepository.GetById(request.Id);
      _logger.Log(LogLevel.Information, $"Account: {account.IdAccount} ,Client {account.IdClient}");

      //Получаем все операции
      _logger.LogInformation("Get operation by account");
      var specification = new BankAccountOperationSpecification(request.Id);
      var operations    = await _operationRepository.ListAsync(specification);
      _logger.LogInformation($"Operation count: {operations.Count()}");

      _logger.LogInformation("Try filtering by date start/end");
      if (request.StartPeriod != null)
        operations = operations.Where(it => it.OperationTime >= request.StartPeriod).ToList();
      if (request.EndPeriod != null)
        operations = operations.Where(it => it.OperationTime <= request.EndPeriod).ToList();
      _logger.LogInformation($"Filtering result: {operations.Count}");

      operations = operations.OrderByDescending(operation => operation.OperationTime).ToList();

      var viewModel = new AccountOperationViewModel
      {
              IdAccount         = request.Id,
              Operations        = operations,
              StartPeriod       = request.StartPeriod,
              EndPeriod         = request.EndPeriod,
              Amount            = account.Amount,
              CanChangeCurrency = account.Deposit.Any() == false && account.Credit.Any() == false,
              IsClosed          = account.DateClose != null
      };
      _logger.LogInformation($"Returned view model: {viewModel}");

      return viewModel;
    }
  }
}