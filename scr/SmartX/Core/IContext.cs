using SmartX.Event;
using SmartX.Logging;
using System.Threading.Tasks;

namespace SmartX.Core
{
    public interface IContext
    {
        Task FireNotifyAsync(NotifyEvent notifyEvent);

        T GetModule<T>() where T : class, IBaseModule;

        ILogger Logger { get; }
    }
}
