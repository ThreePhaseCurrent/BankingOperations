using MediatR;

namespace Web.Commands.Operation
{
    public class TransferAmountCommand : IRequest<bool>
    {
        public int     From   { get; set; }
        public int     To     { get; set; }
        public decimal Amount { get; set; }

        public TransferAmountCommand(int From, int To, decimal Amount)
        {
            this.From   = From;
            this.To     = To;
            this.Amount = Amount;
        }
    }
}