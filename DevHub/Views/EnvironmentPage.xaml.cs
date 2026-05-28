using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DevHub.ViewModels;
using DevHub.Models;

namespace DevHub.Views
{
    public partial class EnvironmentPage : Page
    {
        public EnvironmentViewModel ViewModel { get; }

        public EnvironmentPage()
        {
            ViewModel = new EnvironmentViewModel();
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 🛠️ Extraemos el objeto directamente del Tag de forma 100% segura
            if (sender is Button btn && btn.Tag is EnvironmentAction action)
            {
                ViewModel.ExecuteActionCommand.Execute(action);
            }
        }
    }
}