using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using NOSBlog.Models;

namespace NOSBlog.Auths
{
    public static class UserLogin
    {
        public static int AdminRole
        {
            get
            {
                return 10;
            }
        }
        public static int EmployeeRole
        {
            get
            {
                return 5;
            }
        }
        public static bool IsAdmin {
            get
            {
                if (HttpContext.Current.Session["UserLogin"] != null)
                {
                    NOSBlogEntities context = new NOSBlogEntities();
                    int userId = GetUserLogin.id;
                    user userToCheck = context.users.FirstOrDefault(user => user.id == userId);
                    if (userToCheck == null) return false;
                    return userToCheck.role == AdminRole;
                }
                else
                {
                    return false;
                }
            }
        }
        public static bool IsEmployee
        {
            get
            {
                if (HttpContext.Current.Session["UserLogin"] != null)
                {
                    NOSBlogEntities context = new NOSBlogEntities();
                    int userId = GetUserLogin.id;
                    user userToCheck = context.users.FirstOrDefault(user => user.id == userId);
                    if (userToCheck == null) return false;
                    return userToCheck.role == EmployeeRole;
                }
                else
                {
                    return false;
                }
            }
        }
        public static user GetUserLogin
        {
            get
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
        }
        public static bool IsUserLogin
        {
            get
            {
                return HttpContext.Current.Session["UserLogin"] != null;
            }
        }

        public static void Update(user newUser)
        {
            HttpContext.Current.Session["UserLogin"] = newUser;
        }
    }
}