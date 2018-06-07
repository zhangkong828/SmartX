using System;
using SmartX.Core;
using SmartX.Logging;
using SmartX.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using SmartX.Event;

namespace SmartX.Module
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
