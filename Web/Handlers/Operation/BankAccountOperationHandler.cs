using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BankingSystem.ApplicationCore.Interfaces;

using MediatR;

using Microsoft.Extensions.Logging;

using Web.Commands.Operation;

namespace Web.Handlers.Operation
{
    /// <summary>
    ///   Обработчик для фиксации операций над счетами пользователя
    /// </summary>
    public class BankAccountOperationHandler : AsyncRequestHandler<BankAccountOperationCommand>
    {
        private readonly ILogger<BankAccountOperationHandler> _logger;
        private readonly IAsyncRepository<ApplicationCore.Entity.Operation> _repository;

        public BankAccountOperationHandler(IAsyncRepository<ApplicationCore.Entity.Operation> repository,
                                           ILogger<BankAccountOperationHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        protected override async Task Handle(BankAccountOperationCommand request, CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information,
                        $"Start fixation operation on account {request.IdAccount} at time {DateTime.Now:g}");

            var entity = new ApplicationCore.Entity.Operation
            {
                IdAccount = request.IdAccount,
                OperationTime = DateTime.Now,
                TypeOperation = request.Type,
                Amount = request.Amount
            };

            await _repository.AddAsync(entity);
            _logger.Log(LogLevel.Information, $"Operation {request.Type} was saved successful");
        }
    }
}
