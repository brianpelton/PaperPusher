using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using PaperPusher.ViewModels;

namespace PaperPusher
{
    public class WindowManager : Caliburn.Micro.WindowManager
    {
        #region [ Methods ]

        public override bool? ShowDialog(object rootModel, object context = null,
            IDictionary<string, object> settings = null)
        {
            var window = CreateWindow(rootModel, true, context, settings);
            window.ResizeMode = ResizeMode.NoResize;

            var metrowWindow = window as MetroWindow;
            if (metrowWindow == null)
            {
                return window.ShowDialog();
            }

            var appShell = IoC.Get<MainViewModel>() as IViewAware;
            var appShellWindow = appShell.GetView() as MetroWindow;
            if (appShellWindow == null)
            {
                return metrowWindow.ShowDialog();
            }

            window.Owner = appShellWindow;
            appShellWindow.ShowOverlayAsync();
            var value = metrowWindow.ShowDialog();
            appShellWindow.HideOverlayAsync();
            return value;
        }

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            if (!isDialog)
                return base.EnsureWindow(model, view, isDialog);

            var window = view as MetroWindow;

            if (window == null)
            {
                var userControl = view as UserControl;

                var appShell = IoC.Get<MainViewModel>() as IViewAware;

                //var scrollViewer = new ScrollViewer { Content = view };

                window = new MetroWindow
                {
                    //Content = scrollViewer,
                    Content = view,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                if (userControl != null)
                {
                    window.MinHeight = userControl.MinHeight;
                    window.MinWidth = userControl.MinWidth;
                }


                //scrollViewer.SetValue(View.IsGeneratedProperty, true);
                window.SetValue(View.IsGeneratedProperty, true);
                window.Owner = appShell.GetView() as Window;
                window.ShowCloseButton = false;
                window.ShowMaxRestoreButton = false;
                window.ShowMinButton = false;
                window.GlowBrush = new SolidColorBrush(Colors.Gray);
            }

            return window;
        }

        #endregion
    }
}