using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TwitterApp.Models;
using TwitterInteraction;

namespace TwitterApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbCtx _db = new DbCtx();
        private readonly TwitterUse _twitterUse;
        public HomeController()
        {
            _twitterUse = new TwitterUse();
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Search(string query)
        {
            try
            {
                var json = await _twitterUse.SearchAsync(query);
                var jsonStatuses = json["statuses"];
                var tweets = (from tweet in jsonStatuses
                    let twitterDate = tweet["created_at"].ToString()
                    let dt = DateTimeOffset.ParseExact(twitterDate, 
                                                       "ddd MMM dd HH:mm:ss zzz yyyy", 
                                                       CultureInfo.InvariantCulture)
                    let createdAt = dt.DateTime + dt.Offset
                    select new Tweets
                    {
                        CreatedAt = createdAt, 
                        Text = tweet["text"].ToString(), 
                        UserName = tweet["user"]["name"].ToString(), 
                        ProfileImage = tweet["user"]["profile_image_url"].ToString()
                    }).ToList();
                _db.Tweets.AddRange(tweets);
                _db.SaveChanges();
                return View(tweets);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("~/Views/Home/Error.cshtml");
            }
        }
        public ActionResult History(int page = 1)
        {
            int pageSize = 10;
            IQueryable<Tweets> tweetsPerPage = _db.Tweets.OrderBy(tweet => tweet.Id).Skip((page - 1) * pageSize).Take(pageSize);
            VIewTweetsModel vIewTweetsModel = new VIewTweetsModel
            {
                Tweets = tweetsPerPage,
                PageInfo = new PageInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = _db.Tweets.Count()
                }
            };
            return View(vIewTweetsModel);
        }

        public ActionResult Delete (long id)
        {
            Tweets tweet = _db.Tweets.Find(id);
            _db.Tweets.Remove(tweet);
            _db.SaveChanges();
            return RedirectToAction("History");
        }

        public ActionResult ClearDb()
        {
            List<Tweets> tweets = _db.Tweets.ToList();
            _db.Tweets.RemoveRange(tweets);
            _db.SaveChanges();
            return RedirectToAction("History");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
