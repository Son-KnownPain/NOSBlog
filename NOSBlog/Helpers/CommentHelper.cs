using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NOSBlog.Models;
using NOSBlog.Auths;

namespace NOSBlog.Helpers
{
    public static class CommentHelper
    {
        public static bool IsLikeComment(int commentId)
        {
            if (AuthManager.User.IsUserLogin)
            {
                int userId = AuthManager.User.GetUserLogin.id;
                NOSBlogEntities context = new NOSBlogEntities();
                user_like_comments recordChecking = context.user_like_comments.FirstOrDefault(
                        rd => rd.user_id == userId && rd.comment_id == commentId
                    );
                if (recordChecking != null)
                {
                    return true;
                } else
                {
                    return false;
                }
            } else
            {
                return false;
            }
        }

        public static bool IsYourComment(int commentId)
        {
            if (AuthManager.User.IsUserLogin)
            {
                int userId = AuthManager.User.GetUserLogin.id;
                NOSBlogEntities context = new NOSBlogEntities();
                comment commentChecking = context.comments.FirstOrDefault(cmt => cmt.id == commentId);
                return (commentChecking != null && commentChecking.user_id == userId);
            } else
            {
                return false;
            }
        }
    }
}