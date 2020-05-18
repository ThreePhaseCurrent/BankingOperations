using System;
using MediatR;
using Web.ViewModels;

namespace Web.Commands.Operation
{
    /// <summary>
    ///   Query data class for get account operation with filter by range date
    /// </summary>
    public class GetAccountOperationQuery : IRequest<AccountOperationViewModel>
    {
        public int       Id          { get; }
        public DateTime? StartPeriod { get; }
        public DateTime? EndPeriod   { get; }
        public decimal   Amount      { get; set; }

        public GetAccountOperationQuery(int id, DateTime? startPeriod, DateTime? endPeriod)
        {
            Id          = id;
            StartPeriod = startPeriod;
            EndPeriod   = endPeriod;
        }
    }
}