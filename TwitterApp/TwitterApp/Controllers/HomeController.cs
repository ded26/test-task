using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TwitterApp.Models;
using PagedList;
using PagedList.Mvc;

namespace TwitterApp.Controllers
{
    public class HomeController : Controller
    {
        private DbCtx db = new DbCtx();

        // GET: /Home/
        TwitterAuth twitter;
        public HomeController()
        {
            twitter = new TwitterAuth
            {
                OAuthConsumerKey = "thrdu56pe9GwlGSGivK6k6Zb9",
                OAuthConsumerSecret = "ZpvagWEkois2XMFSHMViKRDZ01Ww2QWklHCepxIHyAKbvcqvsn"
            };
        }


        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Search(string query)
        {
            string accessToken = await GetAccessToken();
            var searchRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.twitter.com/1.1/search/tweets.json?q=" + query);
            searchRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            var httpClient = new HttpClient();
            HttpResponseMessage searchResponse = await httpClient.SendAsync(searchRequest);
            searchResponse.EnsureSuccessStatusCode();
            dynamic responseData = await Task.Run(() => searchResponse.Content.ReadAsStringAsync());
            var serializer = new JavaScriptSerializer();
            dynamic deserializeData = serializer.Deserialize<object>(responseData);
            IEnumerable<dynamic> statuses = (deserializeData["statuses"] as IEnumerable<dynamic>);
            List<Tweets> tweets = new List<Tweets>();
            foreach (var item in statuses)
            {
                dynamic user = item["user"];
                tweets.Add(new Tweets 
                {
                    //Id = item["id"],
                    CreatedAt = item["created_at"],
                    Text = item["text"],
                    UserName = user["name"],
                    ProfileImage = user["profile_image_url"]
                });
            }
            db.Tweets.AddRange(tweets);
            db.SaveChanges();
            return View(db.Tweets.ToList());
        }

        public async Task<string> GetAccessToken()
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token ");
            var customerInfo = Convert.ToBase64String(new UTF8Encoding().GetBytes(twitter.OAuthConsumerKey + ":" + twitter.OAuthConsumerSecret));
            request.Headers.Add("Authorization", "Basic " + customerInfo);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = await httpClient.SendAsync(request);

            string json = await response.Content.ReadAsStringAsync();
            var serializer = new JavaScriptSerializer();
            dynamic item = serializer.Deserialize<object>(json);
            return item["access_token"];
        }

        public ActionResult History(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            List<Tweets> tweets = db.Tweets.ToList<Tweets>();
            return View(tweets.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Delete (long id)
        {
            Tweets tweet = db.Tweets.Find(id);
            db.Tweets.Remove(tweet);
            db.SaveChanges();
            return RedirectToAction("History");
        }

        public ActionResult ClearDB()
        {
            List<Tweets> tweets = db.Tweets.ToList();
            foreach (var tweet in tweets)
            {
                db.Tweets.Remove(tweet);
            }
            db.SaveChanges();
            return RedirectToAction("History");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
