using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
using WinRT.Interop;
using Microsoft.UI.Windowing;
using Windows.UI;

namespace DevHub.Views
{
    public sealed partial class MainWindow : Window
    {
        private AppWindow _appWindow;

        public MainWindow()
        {
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;

            IntPtr windowHandle = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            _appWindow = AppWindow.GetFromWindowId(windowId);

            ActualizarColoresBarraTitulo();

            if (Content is FrameworkElement rootElement)
            {
                rootElement.ActualThemeChanged += RootElement_ActualThemeChanged;
            }
        }

        private void RootElement_ActualThemeChanged(FrameworkElement sender, object args)
        {
            ActualizarColoresBarraTitulo();
        }

        private void ActualizarColoresBarraTitulo()
        {
            if (_appWindow == null) return;

            _appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            _appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var temaActual = ElementTheme.Default;
            if (Content is FrameworkElement rootElement)
            {
                temaActual = rootElement.ActualTheme;
            }

            if (temaActual == ElementTheme.Dark)
            {
                _appWindow.TitleBar.ButtonForegroundColor = Colors.White;
                _appWindow.TitleBar.ButtonHoverForegroundColor = Colors.White;
                _appWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(30, 255, 255, 255);
            }
            else
            {
                _appWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                _appWindow.TitleBar.ButtonHoverForegroundColor = Colors.Black;
                _appWindow.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(30, 0, 0, 0);
            }
        }

        private void MainNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
                return;
            }

            if (args.SelectedItemContainer is NavigationViewItem item)
            {
                string tag = item.Tag.ToString();

                switch (tag)
                {
                    case "github":
                        ContentFrame.Navigate(typeof(GitHubPage));
                        break;
                    case "environment":
                        ContentFrame.Navigate(typeof(EnvironmentPage));
                        break;
                    case "metrics":
                        ContentFrame.Navigate(typeof(MetricsPage));
                        break;
                    case "settings":
                        ContentFrame.Navigate(typeof(SettingsPage));
                        break;
                }
            }
        }
    }
}
