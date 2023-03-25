using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NOSBlog.Models
{
    public class BlogViewModel
    {
        // blog data
        public int id { get; set; }
        public int user_id { get; set; }
        public string title { get; set; }
        public string summary { get; set; }
        public string thumbnail { get; set; }
        public string content { get; set; }
        public int like_count { get; set; }
        public int comment_count { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        // user data
        public string fullname { get; set; }
        public string avatar { get; set; }
        public bool blue_tick { get; set; }
    }
}