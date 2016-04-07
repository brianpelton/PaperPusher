using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using PaperPusher.ViewModels;

namespace PaperPusher
{
    public class ApplicationBootstrapper : BootstrapperBase
    {
        public ApplicationBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            DisplayRootViewFor<MainViewModel>();
        }
    }
}
