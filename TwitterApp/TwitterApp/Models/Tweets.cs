using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TwitterApp.Models
{
    public class Tweets
    {
        [Key]
        public long Id { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
        public string Text { get; set; }
        public string CreatedAt { get; set; }
    }
}