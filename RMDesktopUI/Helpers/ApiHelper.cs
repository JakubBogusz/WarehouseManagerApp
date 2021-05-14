﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RMDesktopUI.Models;

namespace RMDesktopUI.Helpers
{
    public class ApiHelper : IApiHelper
    {
        private HttpClient apiClient;

        public ApiHelper()
        {
            InitializeClient();
        }

        private void InitializeClient()
        {
            string api = ConfigurationManager.AppSettings["api"];

            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri("");
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("grant_type", username),
                new KeyValuePair<string, string>("grant_type", password)
            });

            AuthenticatedUser result = new AuthenticatedUser();

            using (HttpResponseMessage response = await apiClient.PostAsync("/Token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<AuthenticatedUser>();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }

            }

            return result;

        }
    }
}
