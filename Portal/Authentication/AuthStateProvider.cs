using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace Portal.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;

        private readonly ILocalStorageService _localStorageService;

        private readonly IConfiguration _configuration;

        private readonly AuthenticationState _anonymousState;

        private readonly string _authTokenStorageKey;

        public AuthStateProvider(HttpClient httpClient, 
            ILocalStorageService localStorageService,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _configuration = configuration;
            _authTokenStorageKey = _configuration["authTokenStorageKey"];
            _anonymousState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsync<string>(_authTokenStorageKey);

            if (string.IsNullOrWhiteSpace(token))
            {
                return _anonymousState;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(
                new ClaimsPrincipal(new 
                    ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
        }

        public void NotifyUserAuthentication(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new
                    ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"));

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(_anonymousState);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
