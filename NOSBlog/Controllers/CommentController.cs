using NOSBlog.Auths;
using NOSBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NOSBlog.Controllers
{
    public class CommentController : Controller
    {
        // GET: /Comment/Like?commentId=int
        [HttpGet]
        public ActionResult Like(int? commentId)
        {
            if (commentId == null || !UserLogin.IsUserLogin)
            {
                return Json(
                    new
                    {
                        liked = false,
                        message = "ID empty or you need login to like this blog",
                        toast = new
                        {
                            title = "ID empty or you not login",
                            message = "You need give ID of blog or login before like",
                            type = "error"
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            NOSBlogEntities context = new NOSBlogEntities();
            comment commentLiked = context.comments.FirstOrDefault(comment => comment.id == commentId);
            int userId = UserLogin.GetUserLogin.id;
            user userLikeComment = context.users.FirstOrDefault(user => user.id == userId);
            if (commentLiked == null || userLikeComment == null)
            {
                return Json(
                    new
                    {
                        liked = false,
                        message = "Comment or user not found",
                        toast = new
                        {
                            title = "Data not found",
                            message = "Comment or user not found",
                            type = "error"
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            // Check if liked then do nothing
            user_like_comments recordChecking = context.user_like_comments.FirstOrDefault(
                rd => rd.user_id == userLikeComment.id && rd.comment_id == commentLiked.id
            );
            if (recordChecking != null)
            {
                return Json(
                    new
                    {
                        liked = false,
                        message = "You liked that comment",
                        toast = new
                        {
                            title = "You liked",
                            message = "This operation cannot be performed due to a problem",
                            type = "error"
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            user_like_comments recordLikeComment = new user_like_comments();

            recordLikeComment.user_id = userLikeComment.id;
            recordLikeComment.comment_id = commentLiked.id;
            recordLikeComment.created_at = DateTime.Now;

            commentLiked.like_count += 1;
            
            context.user_like_comments.Add(recordLikeComment);

            context.SaveChanges();

            return Json(
                new
                {
                    liked = true
                },
                JsonRequestBehavior.AllowGet
            );
        }

        // GET: /Comment/Unlike?commentId=int
        [HttpGet]
        public ActionResult Unlike(int? commentId)
        {
            // Check if query param is null or unauthorize
            if (commentId == null || !UserLogin.IsUserLogin)
            {
                return Json(
                    new
                    {
                        unliked = false,
                        message = "ID empty or you need login to like this blog",
                        toast = new
                        {
                            title = "ID empty or you not login",
                            message = "You need give ID of blog or login before like",
                            type = "error"
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            NOSBlogEntities context = new NOSBlogEntities();
            comment commentUnliked = context.comments.FirstOrDefault(cmt => cmt.id == commentId);
            int userId = UserLogin.GetUserLogin.id;
            user userUnlikeBlog = context.users.FirstOrDefault(user => user.id == userId);
            // Check if doesn't exist blog or user
            if (commentUnliked == null || userUnlikeBlog == null)
            {
                return Json(
                    new
                    {
                        unliked = false,
                        message = "Comment or user not found",
                        toast = new
                        {
                            title = "Data not found",
                            message = "Comment or user not found",
                            type = "error"
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            // Check if unliked then do nothing
            user_like_comments recordChecking = context.user_like_comments.FirstOrDefault(
                rd => rd.comment_id == commentId && rd.user_id == userId
                );
            if (recordChecking == null)
            {
                return Json(
                    new
                    {
                        unliked = false,
                        message = "You not liked that comment",
                        toast = new
                        {
                            title = "You not liked",
                            message = "This operation cannot be performed due to a problem",
                            type = "error"
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            commentUnliked.like_count -= 1;
            context.user_like_comments.Remove(recordChecking);
            context.SaveChanges();

            return Json(
                new
                {
                    unliked = true
                },
                JsonRequestBehavior.AllowGet
            );
        }

        // GET: /Comment/Remove?commentId=int
        [HttpGet]
        public ActionResult Remove(int? commentId)
        {
            if (commentId != null && UserLogin.IsUserLogin)
            {
                NOSBlogEntities context = new NOSBlogEntities();
                comment commentToRemove = context.comments.FirstOrDefault(cmt => cmt.id == commentId);
                if (commentToRemove == null || commentToRemove.user_id != UserLogin.GetUserLogin.id)
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
                blog blogComment = context.blogs.FirstOrDefault(blog => blog.id == commentToRemove.blog_id);
                if (blogComment == null)
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
                // Not equal null and is your comment
                // => Remove all relationship record first
                // Only remove user_like_comments record
                context.user_like_comments.RemoveRange(context.user_like_comments.Where(rd => rd.comment_id == commentToRemove.id));
                context.comments.Remove(commentToRemove);

                // Decrease comment count
                blogComment.comment_count -= 1;

                context.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}