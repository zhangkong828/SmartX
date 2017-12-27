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
    public class BaseModule : IBaseModule
    {

        public BaseModule(IContext context)
        {
            _context = context;

        }

        public readonly IContext _context;

        public ILogger _logger => _context.Logger;

        public SessionModule _session => _context.GetModule<SessionModule>();
        public StoreModule _store => _context.GetModule<StoreModule>();

        public long _timestamp => DateTime.Now.ToTimestampMilli();

        public HttpClient _httpClient => _session._httpClient;


    }
}
