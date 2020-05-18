using System.Threading;
using System.Threading.Tasks;

namespace Web.Interfaces
{
    public interface ICreditScopedService
    {
        Task DoWork(CancellationToken cancellationToken);
    }
}