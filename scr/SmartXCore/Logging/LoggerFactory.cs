namespace SmartXCore.Logging
{
    public class LoggerFactory
    {
        private static readonly object obj = new object();

        private static ILogger _instance = null;

        public static ILogger Instance
        {
            get
            {
                return _instance ?? CreateLog4NetLogger();
            }
        }


        private static ILogger CreateLog4NetLogger()
        {
            if (_instance == null)
            {
                lock (obj)
                {
                    if (_instance == null)
                    {
                        _instance = new Log4NetLogger();
                    }
                }
            }
            return _instance;
        }


    }
}
