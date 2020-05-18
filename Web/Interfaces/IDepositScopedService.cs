using System.Threading;
using System.Threading.Tasks;

namespace Web.Interfaces
{
    public interface IDepositScopedService
    {
        Task DoWork(CancellationToken cancellationToken);
    }
}