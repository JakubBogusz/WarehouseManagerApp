using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Portal.Models;

namespace Portal.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;

        private readonly AuthenticationStateProvider _authenticationStateProvider;

        private readonly ILocalStorageService _localStorageService;

        private readonly IConfiguration _configuration;

        private readonly string _authTokenStorageKey;

        public AuthenticationService(HttpClient client, AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorageService,
            IConfiguration configuration)
        {
            _client = client;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorageService = localStorageService;
            _configuration = configuration;
            _authTokenStorageKey = _configuration["authTokenStorageKey"];
        }

        public async Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", userForAuthentication.Email),
                new KeyValuePair<string, string>("password", userForAuthentication.Password)

            });

            string api = _configuration["api"] + _configuration["tokenEndpoint"];
            HttpResponseMessage authResult = await _client.PostAsync(api, data);
            var authContent = await authResult.Content.ReadAsStringAsync();

            if (authResult.IsSuccessStatusCode == false)
            {
                return null;
            }

            var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(
                authContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            await _localStorageService.SetItemAsync(_authTokenStorageKey, result.Access_Token);

            ((AuthStateProvider)_authenticationStateProvider).NotifyUserAuthentication(result.Access_Token);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Access_Token);

            return result;
        }

        public async Task Logout()
        {
            await _localStorageService.RemoveItemAsync(_authTokenStorageKey);

            ((AuthStateProvider)_authenticationStateProvider).NotifyUserLogout();
            _client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
