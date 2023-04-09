using NOSBlog.Filters;
using System.Web;
using System.Web.Mvc;

namespace NOSBlog
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // Auto re-login when cookies pass check
            filters.Add(new CookieAuthenticate());
        }
    }
}
