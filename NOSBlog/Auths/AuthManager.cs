using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using NOSBlog.Models;

namespace NOSBlog.Auths
{
    public static class AuthManager
    {
        public static string SECRET_KEY
        {
            get => "hs0226";
        }
        public static string AUTH_USERNAME_KEY
        {
            get => "auth_user";
        }
        public static string AUTH_HASHED_KEY
        {
            get => "auth_hashed";
        }
        public static string GetHashKey(string data)
        {
            return data + SECRET_KEY;
        }
        public static bool Login(user userLogin)
        {
            HttpCookie authUsernameCookie = new HttpCookie(
                AUTH_USERNAME_KEY, 
                userLogin.username
            );
            HttpContext.Current.Response.Cookies.Add(authUsernameCookie);

            HttpCookie authHash = new HttpCookie(
                AUTH_HASHED_KEY,
                Crypto.HashPassword(AuthManager.GetHashKey(userLogin.username))
            );
            HttpContext.Current.Response.Cookies.Add(authHash);

            AuthManager.User.Update(userLogin);

            return true;
        }
        public static bool Logout()
        {
            HttpCookie authUsernameCookie = new HttpCookie(AUTH_USERNAME_KEY);
            HttpCookie authHash = new HttpCookie(AUTH_HASHED_KEY);

            authUsernameCookie.Expires = DateTime.Now.AddDays(-1);
            authHash.Expires = DateTime.Now.AddDays(-1);

            HttpContext.Current.Response.Cookies.Add(authHash);
            HttpContext.Current.Response.Cookies.Add(authUsernameCookie);

            AuthManager.User.RemoveSession();

            return true;
        }
        // Working with session
        public static class User
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
            public static bool IsAdmin
            {
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

            public static void RemoveSession()
            {
                HttpContext.Current.Session.Remove("UserLogin");
            }
        }
    }
}