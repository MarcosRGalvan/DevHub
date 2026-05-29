using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevHub.Models;
using DevHub.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DevHub.ViewModels
{
    public partial class GitHubViewModel : ObservableObject
    {
        private readonly GitHubService _gitHubService;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _showUserNotFoundAlert;

        public ObservableCollection<Repository> Repositories { get; } = new();

        public GitHubViewModel()
        {
            _gitHubService = new GitHubService();
        }

        [RelayCommand]
        private async Task FetchReposAsync()
        {
            if (string.IsNullOrWhiteSpace(Username)) return;

            IsBusy = true;
            ShowUserNotFoundAlert = false;
            Repositories.Clear();

            try
            {
                var repos = await _gitHubService.GetUserRepositoriesAsync(Username);

                foreach (var repo in repos)
                {
                    Repositories.Add(repo);
                }
            }
            catch (HttpRequestException)
            {
                Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
                {
                    ShowUserNotFoundAlert = true;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error de comunicación: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
