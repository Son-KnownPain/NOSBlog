using NOSBlog.Auths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using NOSBlog.Models;

namespace NOSBlog.Filters
{
    public class UserAuthorization : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            HttpCookie authUserCookie = filterContext.HttpContext.Request.Cookies.Get(AuthManager.AUTH_USERNAME_KEY);
            HttpCookie authHashed = filterContext.HttpContext.Request.Cookies.Get(AuthManager.AUTH_HASHED_KEY);

            bool isNullCookies = authUserCookie == null && authHashed == null;
            if (isNullCookies)
            {
                AuthManager.User.Update(null);
                filterContext.Result = new HttpUnauthorizedResult();
            } else
            {
                string username = authUserCookie.Value;
                string codeHashed = authHashed.Value;

                bool isValidAuth = Crypto.VerifyHashedPassword(codeHashed, AuthManager.GetHashKey(username));

                if (!isValidAuth)
                {
                    AuthManager.User.Update(null);
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
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "User" },
                    { "action", "Login" }
                });
            }
        }
    }
}