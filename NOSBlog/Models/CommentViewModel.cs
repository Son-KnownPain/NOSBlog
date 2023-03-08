using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NOSBlog.Models
{
    public class CommentViewModel
    {
        public int comment_id { get; set; }
        public int user_id { get; set; }
        public string content { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public int like_count { get; set; }
        public string fullname { get; set; }
        public bool blue_tick { get; set; }
        public string avatar { get; set; }
    }
}