using System;

namespace PaperPusher
{
    /// <summary>
    /// Allows directing Caliburn's internal logging to log4net's output.
    /// </summary>
    public class CaliburnLog4netLogger : Caliburn.Micro.ILog
    {
        #region [ Fields ]

        private readonly log4net.ILog _logger;

        #endregion

        #region [ Constructors ]

        public CaliburnLog4netLogger(Type type)
        {
            _logger = log4net.LogManager.GetLogger(type);
        }

        #endregion

        #region [ Interface ILog Members ]

        public void Error(Exception exception)
        {
            _logger.Error(exception);
        }

        public void Info(string format, params object[] args)
        {
            //if (!Settings.EnableCaliburnConventionNotAppliedLogging)
            //{
            //    if (format.Contains("Action Convention Not Applied"))
            //        return;
            //}

            _logger.InfoFormat(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        #endregion
    }
}