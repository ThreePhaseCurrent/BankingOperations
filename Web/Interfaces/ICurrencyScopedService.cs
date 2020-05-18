using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ICurrencyScopedService
    {
        Task DoWork(CancellationToken cancellationToken);
    }
}