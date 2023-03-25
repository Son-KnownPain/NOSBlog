using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NOSBlog.Models
{
    public class UserItemViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public string image { get; set; }
        public int collection_points { get; set; }
    }
}