using System;

namespace SmartX.Logging
{

    public interface ILoggerFactory
    {
        
        ILogger Create(string name);
      
        ILogger Create(Type type);
    }
}
