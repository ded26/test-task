using System.Collections.Generic;
using System.Linq;

namespace TwitterApp.Models
{
    public class VIewTweetsModel
    {
        public IQueryable<Tweets> Tweets { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}