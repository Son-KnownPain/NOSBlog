using NOSBlog.Auths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NOSBlog.Filters
{
    public class Authen : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!UserLogin.IsUserLogin)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}