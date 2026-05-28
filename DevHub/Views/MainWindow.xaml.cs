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

namespace DevHub.Views
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            MainNavView.SelectedItem = MainNavView.MenuItems[0];
            ContentFrame.Navigate(typeof(GitHubPage));
        }

        private void MainNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
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
                }
            }
        }
    }
}
