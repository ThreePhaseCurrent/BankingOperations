using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationCore.Entity;

namespace ApplicationCore.Interfaces
{
    public interface ICreditRepository
    {
        IQueryable<Credit> Credits { get; }

        Task AddCredit(Credit credit);
        Task CloseCredit(Credit credit);
    }
}
