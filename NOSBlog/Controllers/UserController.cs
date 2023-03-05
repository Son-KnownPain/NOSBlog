using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NOSBlog.Models;
using System.Web.Helpers;
using NOSBlog.Auths;

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
        public ActionResult Check(user userData)
        {
            // Validation: !ModelState.IsValid
            if (false)
            {
                return View("~/Views/User/Login.cshtml");
                //return Json(new
                //{
                //    success = false,
                //    message = "Invalid form"
                //});
            } else
            {
                NOSBlogEntities context = new NOSBlogEntities();
                user userLogin = context.users.FirstOrDefault(user => user.username == userData.username);
                if (userLogin != null && Crypto.VerifyHashedPassword(userLogin.password, userData.password))
                {
                    Session["UserLogin"] = userLogin;
                    return Redirect("/User/Profile");
                    //return Json(new
                    //{
                    //    success = true,
                    //    message = "Login successfully"
                    //});
                } else
                {
                    return Redirect("/User/Register");
                    //return Json(new
                    //{
                    //    success = false,
                    //    message = "Login fail, username or password incorrect"
                    //});
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

                // And redirect to login page
                return Redirect("/User/Login");
            }
        }

        // GET /User/Logout
        public ActionResult Logout()
        {
            Session.Remove("UserLogin");
            return Redirect("/");
        }

        // GET /User/Profile
        public ActionResult Profile()
        {
            if (!UserLogin.IsUserLogin())
            {
                return Redirect("/");
            }
            else
            {
                NOSBlogEntities context = new NOSBlogEntities();
                int userId = UserLogin.GetUserLogin().id;
                user userLogin = context.users.FirstOrDefault(user => user.id == userId);
                List<blog> blogsOfUser = context.blogs.Where(blog => blog.user_id == userLogin.id).ToList();
                ViewBag.user = userLogin;
                ViewBag.blogs = blogsOfUser;
                return View();
            }
        }

        // POST /User/ChangeAvatar
        [HttpPost]
        public ActionResult ChangeAvatar(HttpPostedFileBase avatarFile)
        {
            if (!UserLogin.IsUserLogin())
            {
                return Redirect("/");
            }
            if (avatarFile != null && avatarFile.ContentLength > 0 && avatarFile.ContentLength <= 32000000)
            {
                user userLogin = UserLogin.GetUserLogin();
                NOSBlogEntities context = new NOSBlogEntities();
                user userToChangeAvt = context.users.FirstOrDefault(user => user.id == userLogin.id);
                if (userToChangeAvt != null)
                {
                    // Lưu file mới vào folder Uploads và lưu name của file avt mới
                    // vào database
                    String prefix = DateTime.Now.ToString("ddMMyyyyHHmmss-ms");
                    String uploadFolderPath = Server.MapPath("~/Uploads");
                    String newAvtFileName = prefix + "-" + avatarFile.FileName;
                    // Name của file avt cũ
                    String oldFileName = userToChangeAvt.avatar;

                    // Change avt and save file to uploads folder
                    userToChangeAvt.avatar = newAvtFileName;
                    avatarFile.SaveAs(uploadFolderPath + '/' + newAvtFileName);

                    // Xóa file avt cũ đi
                    if (System.IO.File.Exists(uploadFolderPath + '/' + oldFileName) && !oldFileName.Equals("default-avatar.jpg"))
                    {
                        System.IO.File.Delete(uploadFolderPath + '/' + oldFileName);
                    }
                }
                UserLogin.Update(userToChangeAvt);
                context.SaveChanges();
            }

            return RedirectToAction("Profile");
        }
    }
}