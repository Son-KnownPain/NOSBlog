using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace NOSBlog.Models
{
    public class SimpleUser
    {
        [DisplayName("Username"), Required(ErrorMessage = "Username is require")]
        public string username { get; set; }
        
        [DisplayName("Password"), Required(ErrorMessage = "Password is require")]
        public string password { get; set; }
    }
}