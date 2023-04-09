using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NOSBlog.Models;
using System.Web.Helpers;
using NOSBlog.Auths;
using System.Text.RegularExpressions;
using NOSBlog.Filters;

namespace NOSBlog.Controllers
{
    public class UserController : Controller
    {
        // GET User/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST User/Check
        [HttpPost]
        public ActionResult Check(SimpleUser userData)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/User/Login.cshtml");
            } else
            {
                NOSBlogEntities context = new NOSBlogEntities();
                user userLogin = context.users.FirstOrDefault(user => user.username == userData.username);
                if (userLogin != null && Crypto.VerifyHashedPassword(userLogin.password, userData.password))
                {
                    AuthManager.Login(userLogin);
                    return Redirect("/User/Profile");
                } else
                {
                    TempData["Error"] = "Username or password incorrect";
                    return Redirect("/User/Login");
                }
            }
        }

        // GET User/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: User/Store
        [HttpPost]
        public ActionResult Store(user userData)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/User/Register.cshtml");
            } else
            {
                NOSBlogEntities context = new NOSBlogEntities();
                user newUser = new user();
                newUser.first_name = userData.first_name;
                newUser.last_name = userData.last_name;
                newUser.username = userData.username;
                newUser.email = userData.email;
                newUser.phone = userData.phone;
                newUser.password = Crypto.HashPassword(userData.password);
                newUser.role = 1;
                newUser.avatar = "default-avatar.jpg";
                newUser.blue_tick = false;
                newUser.coins = 0;
                newUser.collection_points = 0;
                newUser.created_at = DateTime.Now;
                newUser.updated_at = DateTime.Now;

                // Add new user into context and save changes
                context.users.Add(newUser);
                context.SaveChanges();

                TempData["Register"] = "Successfully register account, login right now";

                // And redirect to login page
                return Redirect("/User/Login");
            }
        }

        // GET /User/Logout
        [UserAuthorization]
        public ActionResult Logout()
        {
            AuthManager.Logout();
            return Redirect("/");
        }

        // GET /User/Profile
        [UserAuthorization]
        public ActionResult Profile()
        {
            NOSBlogEntities context = new NOSBlogEntities();
            int userId = AuthManager.User.GetUserLogin.id;
            user userLogin = context.users.FirstOrDefault(user => user.id == userId);
            if (userLogin == null) return RedirectToAction("Login");
            List<blog> blogsOfUser = context.blogs.Where(blog => blog.user_id == userLogin.id).OrderByDescending(blog => blog.id).ToList();
            ViewBag.user = userLogin;
            ViewBag.blogs = blogsOfUser;
            return View();
        }

        // POST /User/ChangeAvatar
        [HttpPost]
        [UserAuthorization]
        public ActionResult ChangeAvatar(HttpPostedFileBase avatarFile)
        {
            if (avatarFile != null && avatarFile.ContentLength > 0 && avatarFile.ContentLength <= 32000000)
            {
                user userLogin = AuthManager.User.GetUserLogin;
                NOSBlogEntities context = new NOSBlogEntities();
                user userToChangeAvt = context.users.FirstOrDefault(user => user.id == userLogin.id);
                if (userToChangeAvt != null)
                {
                    // Lưu file mới vào folder Uploads và lưu name của file avt mới
                    // vào database
                    String prefix = DateTime.Now.ToString("ddMMyyyyHHmmss-ms");
                    String uploadFolderPath = Server.MapPath("~/Uploads");
                    String newAvtFileName;
                    if (Regex.IsMatch(avatarFile.FileName, "\\w+"))
                    {
                        newAvtFileName = prefix + "-" + avatarFile.FileName;
                    } else
                    {
                        Random random = new Random();
                        int randomNumber = random.Next();
                        newAvtFileName = "default" + randomNumber;
                    }
                    // Name của file avt cũ
                    String oldFileName = userToChangeAvt.avatar;

                    // Change avt and save file to uploads folder
                    userToChangeAvt.avatar = newAvtFileName;
                    userToChangeAvt.updated_at = DateTime.Now;
                    avatarFile.SaveAs(uploadFolderPath + '/' + newAvtFileName);

                    // Xóa file avt cũ đi
                    if (System.IO.File.Exists(uploadFolderPath + '/' + oldFileName) && !oldFileName.Equals("default-avatar.jpg"))
                    {
                        System.IO.File.Delete(uploadFolderPath + '/' + oldFileName);
                    }
                }
                AuthManager.User.Update(userToChangeAvt);
                context.SaveChanges();
            }

            return RedirectToAction("Profile");
        }

        // GET /User/LikedBlogs
        [HttpGet]
        [UserAuthorization]
        public ActionResult LikedBlogs()
        {
            NOSBlogEntities context = new NOSBlogEntities();
            int userId = AuthManager.User.GetUserLogin.id;
            List<Int32> listBlogId = context.user_like_blogs.Where(rd => rd.user_id == userId).Select(rd => rd.blog_id).ToList();

            List<blog> likedBlogs = context.blogs.Where(blog => listBlogId.Contains(blog.id)).ToList();
            ViewBag.likedBlogs = likedBlogs;
            return View();
        }

        // GET /User/ChangeInfo
        [HttpGet]
        [UserAuthorization]
        public ActionResult ChangeInfo()
        {
            NOSBlogEntities context = new NOSBlogEntities();
            int userId = AuthManager.User.GetUserLogin.id;
            user userToUpdate = context.users.FirstOrDefault(u => u.id == userId);
            if (userToUpdate == null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            return View(userToUpdate);
        }

        // PUT /User/UpdateInfo
        [HttpPut]
        [UserAuthorization]
        public ActionResult UpdateInfo(user userData)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/User/ChangeInfo.cshtml");
            }
            NOSBlogEntities context = new NOSBlogEntities();
            user userToUpdate = context.users.FirstOrDefault(user => user.id == userData.id);
            if (userToUpdate == null) return Redirect("/User/ChangeInfo");
            userToUpdate.first_name = userData.first_name;
            userToUpdate.last_name = userData.last_name;
            userToUpdate.username = userData.username;
            userToUpdate.email = userData.email;
            userToUpdate.phone = userData.phone;
            userToUpdate.password = Crypto.HashPassword(userData.password);
            userToUpdate.updated_at = DateTime.Now;
            context.SaveChanges();

            AuthManager.User.Update(userToUpdate);
            return Redirect("/User/Profile");
        }

        // GET /User/MyItems
        [HttpGet]
        [UserAuthorization]
        public ActionResult MyItems()
        {
            int userId = AuthManager.User.GetUserLogin.id;
            NOSBlogEntities context = new NOSBlogEntities();
            List<UserItemViewModel> items = (from i in context.items
                                             join uic in context.user_item_collections on i.id equals uic.item_id
                                             where uic.user_id == userId
                                             select new UserItemViewModel
                                             {
                                                 id = i.id,
                                                 name = i.name,
                                                 price = uic.price,
                                                 image = i.image,
                                                 collection_points = uic.collection_points,
                                             }).ToList();

            ViewBag.items = items;

            return View();
        }

        // GET /User/ViewItems
        [HttpGet]
        public ActionResult ViewItems(int? userId)
        {
            if (userId == null) return RedirectToAction(Request.UrlReferrer == null ? "/" : Request.UrlReferrer.ToString());
            NOSBlogEntities context = new NOSBlogEntities();
            List<UserItemViewModel> items = (from i in context.items
                                             join uic in context.user_item_collections on i.id equals uic.item_id
                                             where uic.user_id == userId
                                             select new UserItemViewModel
                                             {
                                                 id = i.id,
                                                 name = i.name,
                                                 price = uic.price,
                                                 image = i.image,
                                                 collection_points = uic.collection_points,
                                             }).ToList();

            ViewBag.items = items;

            return View();
        }

        // GET /User/Info
        [HttpGet]
        public ActionResult Info(int? userId)
        {
            if (AuthManager.User.IsUserLogin && userId == AuthManager.User.GetUserLogin.id) return RedirectToAction("Profile");
            if (userId == null) return Redirect(Request.UrlReferrer == null ? "/" : Request.UrlReferrer.ToString());
            NOSBlogEntities context = new NOSBlogEntities();
            user userLogin = context.users.FirstOrDefault(user => user.id == userId);
            if (userLogin == null) return RedirectToAction("Login");
            List<blog> blogsOfUser = context.blogs.Where(blog => blog.user_id == userLogin.id).OrderByDescending(blog => blog.id).ToList();
            ViewBag.user = userLogin;
            ViewBag.blogs = blogsOfUser;
            return View();
        }
    }
}