using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TwitterInteraction
{
    /// <summary>
    /// Simple class to use TwitterApi for search tweets
    /// </summary>
    public class TwitterUse
    {
        private const string OAuthConsumerKey = "thrdu56pe9GwlGSGivK6k6Zb9";
        private const string OAuthConsumerSecret = "ZpvagWEkois2XMFSHMViKRDZ01Ww2QWklHCepxIHyAKbvcqvsn";
        
        private HttpClient _httpClient;
        private HttpRequestMessage _httpRequestMessage;
        private HttpResponseMessage _httpResponseMessage;

        private async Task<string> GetAccessTokenAsync()
        {
            using (_httpClient = new HttpClient())
            {
                using (_httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token "))
                {
                    var customerInfo = Convert.ToBase64String(new UTF8Encoding().GetBytes(OAuthConsumerKey + ":" + OAuthConsumerSecret));
                    _httpRequestMessage.Headers.Add("Authorization", "Basic " + customerInfo);
                    _httpRequestMessage.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                    using (_httpResponseMessage = await _httpClient.SendAsync(_httpRequestMessage))
                    {
                        _httpResponseMessage.EnsureSuccessStatusCode();
                        string json = await _httpResponseMessage.Content.ReadAsStringAsync();
                        string accessToken = JObject.Parse(json).GetValue("access_token").ToString();
                        return accessToken;
                    }
                }
            }
        }

        /// <summary>
        /// Search tweets using Application-Only method as an asynchronous operation
        /// </summary>
        /// <param name="query">Query to search tweets</param>
        /// <returns>Task Jobject</returns>
        public async Task<JObject> SearchAsync (string query)
        {
            string accessToken = await GetAccessTokenAsync();
            using (_httpClient = new HttpClient())
            {
                using (_httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.twitter.com/1.1/search/tweets.json?q=" + query))
                {
                    _httpRequestMessage.Headers.Add("Authorization", "Bearer " + accessToken);
                    using (_httpResponseMessage = await _httpClient.SendAsync(_httpRequestMessage))
                    {
                        _httpResponseMessage.EnsureSuccessStatusCode();
                        string json = await _httpResponseMessage.Content.ReadAsStringAsync();
                        return JObject.Parse(json);
                    }
                }
            }
        }
    }
}
