using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartXCore.Core;
using SmartXCore.Event;
using SmartXCore.Logging;
using SmartXCore.Module;
using Autofac;
using Autofac.Core;

namespace SmartXCore
{
    public class WebWeChatClient : IContext
    {
        private readonly ContainerBuilder _builder;
        private IContainer _container;

        private readonly NotifyEventListener _notifyListener;
        private readonly ILogger _logger;

        public static WebWeChatClient Build(NotifyEventListener notifyListener)
        {
            return new WebWeChatClient(notifyListener);
        }

        public WebWeChatClient(NotifyEventListener notifyListener)
            : this()
        {
            //注册上下文
            _builder.RegisterType<WebWeChatClient>().As<IContext>().SingleInstance();
            //注册日志
            _builder.RegisterType<Log4NetLogger>().As<ILogger>().SingleInstance();
            //注册模块
            _builder.RegisterType<SessionModule>().SingleInstance();
            _builder.RegisterType<LoginModule>().As<ILoginModule>().SingleInstance();
            //注册事件
            _builder.RegisterInstance(notifyListener);

            _container = _builder.Build();

            _logger = _container.Resolve<ILogger>();

            _notifyListener = _container.Resolve<NotifyEventListener>();
        }

        private WebWeChatClient()
        {
            _builder = new ContainerBuilder();

        }

        public ILogger Logger => _logger;

        public T GetModule<T>() where T : class, IBaseModule
        {
            return _container.Resolve<T>();
        }

        public async Task FireNotifyAsync(NotifyEvent notifyEvent)
        {
            try
            {
                if (_notifyListener != null)
                {
                    await _notifyListener(this, notifyEvent);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("FireNotify Error", ex);
            }
        }
        public void Dispose()
        {

        }


        public void Start()
        {
            var loginModule = GetModule<ILoginModule>();
            if (loginModule.Login())
            {
                loginModule.BeginSyncCheck();
            }
        }


    }
}
