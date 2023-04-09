using NOSBlog.Auths;
using NOSBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace NOSBlog.Filters
{
    public class CookieAuthenticate : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookie authUserCookie = filterContext.HttpContext.Request.Cookies.Get(AuthManager.AUTH_USERNAME_KEY);
            HttpCookie authHashed = filterContext.HttpContext.Request.Cookies.Get(AuthManager.AUTH_HASHED_KEY);

            if (authUserCookie != null && authHashed != null && !AuthManager.User.IsUserLogin)
            {
                string username = authUserCookie.Value;
                string codeHashed = authHashed.Value;

                bool isValidAuth = Crypto.VerifyHashedPassword(codeHashed, AuthManager.GetHashKey(username));
                if (isValidAuth)
                {
                    NOSBlogEntities context = new NOSBlogEntities();
                    user userLogin = context.users.FirstOrDefault(user => user.username == username);
                    if (userLogin != null)
                    {
                        AuthManager.User.Update(userLogin);
                    }
                }
            }
        }
    }
}