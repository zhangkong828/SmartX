using SmartXCore.Event;
using SmartXCore.Logging;
using System.Threading.Tasks;

namespace SmartXCore.Core
{
    public interface IContext
    {
        Task FireNotifyAsync(NotifyEvent notifyEvent);

        T GetModule<T>() where T : class, IBaseModule;

        ILogger Logger { get; }
    }
}
