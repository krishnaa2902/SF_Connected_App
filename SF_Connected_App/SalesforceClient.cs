using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SF_Connected_App
{
    public class SalesforceClient
    {
        private readonly IConfiguration _configuration;
        private readonly string clientSecret;
        private readonly string clientId;
        private readonly string username;
        private readonly string password;
        private readonly string tokenUrl;
        private string accessToken;
        private string instanceUrl;

        public SalesforceClient()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();

            clientId = _configuration["Salesforce:ClientId"];
            clientSecret = _configuration["Salesforce:ClientSecret"];
            username = _configuration["Salesforce:Username"];
            password = _configuration["Salesforce:Password"];
            tokenUrl = _configuration["Salesforce:TokenUrl"];

        }

        public async Task<string> GetAccessTokenAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

                var parameters = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password)
                });

                request.Content = parameters;

                var response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                var json = JObject.Parse(content);
                if (json["access_token"] != null)
                {
                    accessToken = json["access_token"].ToString();
                    instanceUrl = json["instance_url"].ToString();
                    Console.WriteLine("Access Token: " + accessToken);
                    Console.WriteLine("Instance URL: " + instanceUrl);
                    return accessToken;
                }
                else
                {
                    Console.WriteLine("Error: " + content);
                    return null;
                }
            }
        }

        public async Task MakeApiCallAsync()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.GetAsync($"{instanceUrl}/services/data/v58.0/sobjects/Account/");
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine("API Response: " + content);
            }
        }











    }  
}      
       