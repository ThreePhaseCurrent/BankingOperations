using MediatR;

namespace Web.Commands.Operation
{
    public class TransferValidationCommand : IRequest<bool>
    {
        public int AccountId { get; set; }

        public TransferValidationCommand(int accountId) => AccountId = accountId;
    }
}