using System;
using SmartXCore.Core;
using SmartXCore.Logging;
using SmartXCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using SmartXCore.Event;

namespace SmartXCore.Module
{
    public abstract class BaseModule : IBaseModule
    {
        public BaseModule()
        {

        }

        public BaseModule(IContext context)
        {
            _context = context;

        }

        protected readonly IContext _context;

        protected ILogger _logger => _context.Logger;

        protected SessionModule _session => _context.GetModule<SessionModule>();

        protected long _timestamp => DateTime.Now.ToTimestampMilli();

        protected HttpClient _httpClient => _session._httpClient;


    }
}
