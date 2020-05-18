using MediatR;

namespace Web.Commands.Deposit
{
    public class ApplyDepositCommand : IRequest<bool>
    {
        public ApplicationCore.Entity.Deposit Deposit { get; set; }

        public ApplyDepositCommand(ApplicationCore.Entity.Deposit deposit)
        {
            Deposit = deposit;
        }
    }
}