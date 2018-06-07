using Autofac;
using SmartX.Core;
using SmartX.Event;
using SmartX.Logging;
using SmartX.Module;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartX
{
    public class WebWeChatClient : IContext
    {
        private readonly ContainerBuilder _builder;
        private IContainer _container;

        private readonly NotifyEventListener _notifyListener;
        private readonly ILogger _logger;

        public static WebWeChatClient Build(NotifyEventListener notifyListener, ILogger logger = null)
        {
            if (logger == null)
                logger = new EmptyLogger();
            return new WebWeChatClient(notifyListener, logger);
        }

        private WebWeChatClient(NotifyEventListener notifyListener, ILogger logger)
            : this()
        {
            //注册日志
            _builder.RegisterInstance(logger).As<ILogger>().SingleInstance();
            //注册模块
            _builder.RegisterInstance(new SessionModule()).SingleInstance();
            _builder.RegisterInstance(new StoreModule()).SingleInstance();
            _builder.Register<ILoginModule>(x => new LoginModule(this)).SingleInstance();
            _builder.Register<IContactModule>(x => new ContactModule(this)).SingleInstance();
            _builder.Register<IChatModule>(x => new ChatModule(this)).SingleInstance();
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
            //
        }


        public bool Start()
        {
            var loginModule = GetModule<ILoginModule>();
            if (loginModule.Login())
            {
                loginModule.BeginSyncCheck();
                return true;
            }
            else
                return false;
        }

        public void Stop()
        {
            GetModule<ILoginModule>().Logout();
        }
    }
}
