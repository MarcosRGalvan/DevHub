using DevHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DevHub.Services
{
    public class GitHubService
    {
        private readonly HttpClient _httpClient;

        public GitHubService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.github.com/")
            };

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DevHubApp");
        }

        public async Task<List<Repository>> GetUserRepositoriesAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Repository>>($"users/{username}/repos?sort=updated");
                return response ?? new List<Repository>();
            }
            catch (Exception)
            {
                return new List<Repository>();
            }
        }
    }
}
