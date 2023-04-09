using NOSBlog.Auths;
using NOSBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace NOSBlog.Filters
{
    public class AdminAuthorization : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            HttpCookie authUserCookie = filterContext.HttpContext.Request.Cookies.Get(AuthManager.AUTH_USERNAME_KEY);
            HttpCookie authHashed = filterContext.HttpContext.Request.Cookies.Get(AuthManager.AUTH_HASHED_KEY);

            bool isNullCookies = authUserCookie == null && authHashed == null;
            if (isNullCookies)
            {
                AuthManager.User.RemoveSession();
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                string username = authUserCookie.Value;
                string codeHashed = authHashed.Value;

                bool isValidAuth = Crypto.VerifyHashedPassword(codeHashed, AuthManager.GetHashKey(username));

                if (!isValidAuth)
                {
                    AuthManager.User.RemoveSession();
                    filterContext.Result = new HttpUnauthorizedResult();
                }
                else if (!AuthManager.User.IsUserLogin)
                {
                    NOSBlogEntities context = new NOSBlogEntities();
                    user userLogin = context.users.FirstOrDefault(user => user.username == username);
                    if (userLogin != null)
                    {
                        AuthManager.User.Update(userLogin);
                    }
                }
                if (!AuthManager.User.IsAdmin) filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectResult("/");
            }
        }
    }
}