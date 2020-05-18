using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using BankingSystem.ApplicationCore.Interfaces;
using MediatR;

namespace Web.Commands.Operation
{
    public class CheckAccountExistQuery : IRequest<bool>
    {
        public int Id { get; set; }

        public CheckAccountExistQuery(int id) => Id = id;
    }

    public class CheckAccountExistHandler : IRequestHandler<CheckAccountExistQuery, bool>
    {
        private readonly IAsyncRepository<BankAccount> _bankAccountRepository;

        public CheckAccountExistHandler(IAsyncRepository<BankAccount> bankAccountRepository) =>
            _bankAccountRepository = bankAccountRepository;

        public async Task<bool> Handle(CheckAccountExistQuery request, CancellationToken cancellationToken)
        {
            var account = await _bankAccountRepository.GetById(request.Id);

            return account != null;
        }
    }
}