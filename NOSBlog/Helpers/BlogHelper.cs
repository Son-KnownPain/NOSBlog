using NOSBlog.Auths;
using NOSBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NOSBlog.Helpers
{
    public static class BlogHelper
    {
        public static string IsLiked(int blogId)
        {
            if (AuthManager.User.IsUserLogin)
            {
                NOSBlogEntities context = new NOSBlogEntities();
                int userId = AuthManager.User.GetUserLogin.id;
                user_like_blogs recordChecking = context.user_like_blogs.FirstOrDefault(
                    rd => rd.user_id == userId && rd.blog_id == blogId
                );
                if (recordChecking != null)
                {
                    return "liked";
                } else
                {
                    return "";
                }
            } else
            {
                return "";
            }
        }
    }
}