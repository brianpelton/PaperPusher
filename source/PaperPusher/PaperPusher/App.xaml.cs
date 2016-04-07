using System.Windows;
using log4net;
using log4net.Config;

[assembly: XmlConfigurator]

namespace PaperPusher
{
    public partial class App : Application
    {
        #region [ Logging ]

        private static readonly ILog Log
            = LogManager.GetLogger(typeof (App));

        #endregion

        #region [ Methods ]

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Log.Info("Application Exit.");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Log.Info("Application Startup.");
        }

        #endregion
    }
}