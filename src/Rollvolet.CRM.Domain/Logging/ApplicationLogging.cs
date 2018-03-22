using Microsoft.Extensions.Logging;

namespace Rollvolet.CRM.Domain.Logging
{
    public class ApplicationLogging
    {
        private static ILoggerFactory _Factory = null;

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new LoggerFactory();
                }
                return _Factory;
            }
            set { _Factory = value; }
        }

        public static ILogger CreateLogger(string category) => LoggerFactory.CreateLogger(category);
    }
}