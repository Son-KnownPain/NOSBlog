using NOSBlog.Auths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NOSBlog.Models;
using NOSBlog.Filters;
using System.IO;

namespace NOSBlog.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog/Detail?blogId=int
        [HttpGet]
        public ActionResult Detail(int? blogId)
        {
            if (blogId == null)
            {
                return Redirect("/");
            } else
            {
                NOSBlogEntities context = new NOSBlogEntities();
                blog blogDetail = context.blogs.FirstOrDefault(blog => blog.id == blogId);
                if (blogDetail == null)
                {
                    return Redirect("/");
                }
                user userPosted = context.users.FirstOrDefault(user => user.id == blogDetail.user_id);
                if (userPosted == null)
                {
                    return Redirect("/");
                }
                // Check user login has like that blog
                if (AuthManager.User.IsUserLogin)
                {
                    int userId = AuthManager.User.GetUserLogin.id;
                    user userLogin = context.users.FirstOrDefault(user => user.id == userId);
                    if (userLogin != null)
                    {
                        user_like_blogs userLoginLiked = context.user_like_blogs.FirstOrDefault(
                        rd => rd.user_id == userLogin.id && rd.blog_id == blogDetail.id
                        );
                        ViewBag.isUserLoginLiked = userLoginLiked != null;
                    }
                } else
                {
                    ViewBag.isUserLoginLiked = false;
                }
                // Query list comments
                ViewBag.comments = (from c in context.comments
                                    join u in context.users on c.user_id equals u.id
                                    orderby c.like_count descending, c.id descending
                                    where c.blog_id == blogDetail.id
                                    select new CommentViewModel
                                    {
                                        comment_id = c.id,
                                        user_id = u.id,
                                        content = c.content,
                                        created_at = c.created_at,
                                        like_count = c.like_count,
                                        fullname = u.last_name + " " + u.first_name,
                                        blue_tick = u.blue_tick,
                                        avatar = u.avatar,
                                    }).ToList();

                ViewBag.blog = blogDetail;
                ViewBag.user = userPosted;
                return View();
            }
        }

        // GET: /Blog/Write
        [HttpGet]
        [UserAuthorization]
        public ActionResult Write()
        {
            return View();
        }

        // POST: /Blog/New
        [HttpPost, ValidateInput(false)]
        [UserAuthorization]
        public ActionResult New(blog blogData, HttpPostedFileBase thumbnailFile)
        {
            // Check valid form
            if (!ModelState.IsValid || blogData.content == null)
            {
                return View("~/Views/Blog/Write.cshtml");
            }
            // Check thumbnail
            if (thumbnailFile != null && thumbnailFile.ContentLength > 0 && thumbnailFile.ContentLength <= 32000000)
            {
                // Handle thumbnail file
                String prefix = DateTime.Now.ToString("ddMMyyyyHHmmss-ms_");
                String uploadFolderPath = Server.MapPath("~/Uploads");

                String extName = Path.GetExtension(thumbnailFile.FileName);

                Random random = new Random();
                int randomNumber = random.Next();
                String newFileName = prefix + "blog-img" + randomNumber + extName;

                thumbnailFile.SaveAs(uploadFolderPath + "/" + newFileName);


                // Insert data
                int userId = AuthManager.User.GetUserLogin.id;
                NOSBlogEntities context = new NOSBlogEntities();
                user userPost = context.users.FirstOrDefault(user => user.id == userId);
                blog newBlog = new blog();

                newBlog.user_id = userPost.id;
                newBlog.title = blogData.title;
                newBlog.summary = blogData.summary;
                newBlog.content = blogData.content;
                newBlog.thumbnail = newFileName;
                newBlog.like_count = 0;
                newBlog.comment_count = 0;
                newBlog.@lock = false;
                newBlog.created_at = DateTime.Now;
                newBlog.updated_at = DateTime.Now;

                context.blogs.Add(newBlog);
                context.SaveChanges();

                TempData["PostNewBlog"] = "You have successfully uploaded your blog, please review the blog below";

                return Redirect("/User/Profile");
            }
            else
            {
                return View("~/Views/Blog/Write.cshtml");
            }
        }

        // GET: /Blog/Remove?blogId=int
        [UserAuthorization]
        public ActionResult Remove(int? blogId)
        {
            if (blogId == null)
            {
                return null;
            } else
            {
                // Check if that blog belong user login
                NOSBlogEntities context = new NOSBlogEntities();
                int userId = AuthManager.User.GetUserLogin.id;
                blog blogToRemove = context.blogs.FirstOrDefault(blog => blog.id == blogId);
                user userLogin = context.users.FirstOrDefault(user => user.id == userId);
                if (blogToRemove == null || userLogin == null)
                {
                    return null;
                }
                if (userLogin.id == blogToRemove.user_id)
                {
                    // Remove relationship record of blog
                    // 1. Remove user_like_comments records
                    List<Int32> listCmtIDInBlog = context.comments.Where(cmt => cmt.blog_id == blogToRemove.id)
                        .Select(cmt => cmt.id).ToList();
                    context.user_like_comments.RemoveRange(context.user_like_comments.Where(rd => listCmtIDInBlog.Contains(rd.comment_id)));
                        // 2. Remove user_like_blogs records
                    context.user_like_blogs.RemoveRange(context.user_like_blogs.Where(cmt => cmt.blog_id == blogToRemove.id));
                        // 3. Remove comments of blog
                    context.comments.Where(cmt => cmt.blog_id == blogToRemove.id).ToList()
                        .ForEach(cmt => context.comments.Remove(cmt));
                    // Remove thumbnail of blog
                    String uploadsFolderPath = Server.MapPath("~/Uploads");
                    if (System.IO.File.Exists(uploadsFolderPath + "/" + blogToRemove.thumbnail))
                    {
                        System.IO.File.Delete(uploadsFolderPath + "/" + blogToRemove.thumbnail);
                    }
                    // Remove record in database
                    context.blogs.Remove(blogToRemove);
                    context.SaveChanges();

                    TempData["RemoveBlog"] = "Successfully remove your blog";

                    return Redirect("/User/Profile");
                } else
                {
                    return null;
                }
            }
        }

        // GET: /Blog/Like?blogId=int
        [HttpGet]
        public ActionResult Like(int? blogId)
        {
            if (blogId == null || !AuthManager.User.IsUserLogin)
            {
                return Json(
                    new {
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
            blog blogLiked = context.blogs.FirstOrDefault(blog => blog.id == blogId);
            int userId = AuthManager.User.GetUserLogin.id;
            user userLikeBlog = context.users.FirstOrDefault(user => user.id == userId);
            if (blogLiked == null || userLikeBlog == null)
            {
                return Json(
                    new
                    {
                        liked = false,
                        message = "Blog or user not found",
                        toast = new
                        {
                            title = "Data not found",
                            message = "Blog or user not found",
                            type = "error"
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            // Check if liked then do nothing
            user_like_blogs recordChecking = context.user_like_blogs.FirstOrDefault(
                rd => rd.user_id == userLikeBlog.id && rd.blog_id == blogLiked.id
                );
            if (recordChecking != null)
            {
                return Json(
                    new
                    {
                        liked = false,
                        message = "You liked that blog",
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

            user_like_blogs recordLikeBlog = new user_like_blogs();
            recordLikeBlog.user_id = userLikeBlog.id;
            recordLikeBlog.blog_id = blogLiked.id;
            recordLikeBlog.created_at = DateTime.Now;

            blogLiked.like_count += 1;

            context.user_like_blogs.Add(recordLikeBlog);
            context.SaveChanges();

            return Json(
                new
                {
                    liked = true
                },
                JsonRequestBehavior.AllowGet
            );
        }

        // GET: /Blog/Unlike?blogId=int
        [HttpGet]
        public ActionResult Unlike(int? blogId)
        {
            // Check if query param is null or unauthorize
            if (blogId == null || !AuthManager.User.IsUserLogin)
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
            blog blogUnliked = context.blogs.FirstOrDefault(blog => blog.id == blogId);
            int userId = AuthManager.User.GetUserLogin.id;
            user userUnlikeBlog = context.users.FirstOrDefault(user => user.id == userId);
            // Check if doesn't exist blog or user
            if (blogUnliked == null || userUnlikeBlog == null)
            {
                return Json(
                    new
                    {
                        unliked = false,
                        message = "Blog or user not found",
                        toast = new
                        {
                            title = "Data not found",
                            message = "Blog or user not found",
                            type = "error"
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            // Check if unliked then do nothing
            user_like_blogs recordChecking = context.user_like_blogs.FirstOrDefault(
                rd => rd.user_id == userUnlikeBlog.id && rd.blog_id == blogUnliked.id
                );
            if (recordChecking == null)
            {
                return Json(
                    new
                    {
                        unliked = false,
                        message = "You not liked that blog",
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

            blogUnliked.like_count -= 1;

            context.user_like_blogs.Remove(recordChecking);
            context.SaveChanges();

            return Json(
                new
                {
                    unliked = true
                },
                JsonRequestBehavior.AllowGet
            );
        }

        // POST: /Blog/Comment?blogId=int
        [HttpPost]
        public ActionResult Comment(int? blogId, String content)
        {
            
            // Check is valid form
            if (!ModelState.IsValid && !content.Equals(""))
            {
                return Json(
                    new
                    {
                        commented = false,
                        message = "Invalid comment input value",
                        toast = new
                        {
                            title = "Empty comment",
                            message = "Comment must be not empty",
                            type = "error"
                        }
                    }
                );
            }
            // Check id and user is login
            if (blogId == null || !AuthManager.User.IsUserLogin)
            {
                return Json(new
                {
                    commented = false,
                    message = "ID not exist or you not login",
                    toast = new
                    {
                        title = "Error",
                        message = "You are not login or blog doesn't exist",
                        type = "error"
                    }
                });
            }

            NOSBlogEntities context = new NOSBlogEntities();
            blog blogCmt = context.blogs.FirstOrDefault(blog => blog.id == blogId);
            int userId = AuthManager.User.GetUserLogin.id;
            user userCmt = context.users.FirstOrDefault(user => user.id == userId);
            // Check null
            if (userCmt == null || blogCmt == null)
            {
                return Json(new
                {
                    commented = false,
                    message = "User or blog not found",
                    toast = new
                    {
                        title = "Error",
                        message = "User or blog not found",
                        type = "error"
                    }
                });
            }

            // new comment and cmt count + 1
            blogCmt.comment_count += 1;

            comment newComment = new comment();
            newComment.user_id = userCmt.id;
            newComment.blog_id = blogCmt.id;
            newComment.content = content;
            newComment.like_count = 0;
            newComment.created_at = DateTime.Now;
            newComment.updated_at = DateTime.Now;
            context.comments.Add(newComment);
            context.SaveChanges();


            return Json(new
            {
                commented = true,
                userComment = new
                {
                    user_id = userCmt.id,
                    comment_id = newComment.id,
                    name = userCmt.last_name + " " + userCmt.first_name,
                    blue_tick = userCmt.blue_tick,
                    avatar = userCmt.avatar,
                    created_at = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ms"),
                    heart = newComment.like_count
                }
            });
        }

        // GET: /Blog/Lock?blogId=int
        [HttpGet]
        [UserAuthorization]
        public ActionResult Lock(int? blogId)
        {
            if (!ModelState.IsValid) return Redirect("/User/Profile");
            // if not give me blog id or still not login
            if (blogId == null)
            {
                return Redirect("/User/Profile");
            } 
            else
            {
                NOSBlogEntities context = new NOSBlogEntities();
                blog blogToLock = context.blogs.FirstOrDefault(blog => blog.id == blogId);
                int userId = AuthManager.User.GetUserLogin.id;
                // blog found and lock = false and that your blog
                if (blogToLock != null && !blogToLock.@lock && blogToLock.user_id == userId)
                {
                    user userGetCoins = context.users.FirstOrDefault(user => user.id == userId);
                    if (userGetCoins != null)
                    {
                        userGetCoins.coins += blogToLock.like_count;
                    }
                    else
                    {
                        return Redirect("/User/Profile");
                    }
                    blogToLock.@lock = true;
                    context.SaveChanges();
                }
                //try
                //{
                    
                //}
                //catch (DbEntityValidationException e)
                //{
                //    foreach (var eve in e.EntityValidationErrors)
                //    {
                //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                //        foreach (var ve in eve.ValidationErrors)
                //        {
                //            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                //                ve.PropertyName, ve.ErrorMessage);
                //        }
                //    }
                //}

                return Redirect("/User/Profile");
            }
        }
    }
}