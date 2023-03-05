using Microsoft.Ajax.Utilities;
using NOSBlog.Auths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NOSBlog.Models;
using Microsoft.SqlServer.Server;

namespace NOSBlog.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog/Detail
        [HttpGet]
        public ActionResult Detail()
        {
            return View();
        }

        // GET: /Blog/Write
        [HttpGet]
        public ActionResult Write()
        {
            if (!UserLogin.IsUserLogin())
            {
                return Redirect("/User/Login");
            }
            return View();
        }

        // POST: /Blog/New
        [HttpPost, ValidateInput(false)]
        public ActionResult New(blog blogData, HttpPostedFileBase thumbnailFile)
        {
            // Check is login
            if (!UserLogin.IsUserLogin())
            {
                return Redirect("/User/Login");
            } else
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
                    String newFileName = prefix + thumbnailFile.FileName;
                    String uploadFolderPath = Server.MapPath("~/Uploads");
                    thumbnailFile.SaveAs(uploadFolderPath + "/" + newFileName);


                    // Insert data
                    int userId = UserLogin.GetUserLogin().id;
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
                } else
                {
                    return View("~/Views/Blog/Write.cshtml");
                }
            }
        }
    }
}