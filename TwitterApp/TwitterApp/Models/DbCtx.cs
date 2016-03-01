using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TwitterApp.Models
{
    public class DbCtx : DbContext
    {
        public System.Data.Entity.DbSet<Tweets> Tweets { get; set; }
    }
}