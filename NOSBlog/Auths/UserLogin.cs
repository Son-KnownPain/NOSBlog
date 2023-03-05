using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NOSBlog.Models;

namespace NOSBlog.Auths
{
    public static class UserLogin
    {
        public static user GetUserLogin()
        {
            if (HttpContext.Current.Session["UserLogin"] != null)
            {
                return HttpContext.Current.Session["UserLogin"] as user;
            }
            else
            {
                return null;
            }
        }
        public static bool IsUserLogin()
        {
            return HttpContext.Current.Session["UserLogin"] != null;
        }

        public static void Update(user newUser)
        {
            HttpContext.Current.Session["UserLogin"] = newUser;
        }
    }
}