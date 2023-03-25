using NOSBlog.Auths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NOSBlog.Models;
using NOSBlog.Models.Admin;

namespace NOSBlog.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private NOSBlogEntities context = new NOSBlogEntities();

        // GET: Admin/User/Index
        [HttpGet]
        public ActionResult Index()
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            List<user> allUsers = context.users.ToList();
            ViewBag.users = allUsers;
            return View();
        }

        // GET: admin/user/edit?userId=int
        [HttpGet]
        public ActionResult Edit(int? userId)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            if (userId == null) return RedirectToAction("Index");
            user userEdit = context.users.FirstOrDefault(user => user.id == userId);
            if (userEdit == null) return RedirectToAction("Index");
            return View(userEdit);
        }

        // PUT: admin/user/update
        [HttpPut]
        public ActionResult Update(EditUserViewModel userData)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            if (!ModelState.IsValid) return View("Edit");
            user userUpdated = context.users.FirstOrDefault(user => user.id == userData.id);
            if (userUpdated == null) return Redirect(Request.UrlReferrer.ToString());
            userUpdated.first_name = userData.first_name;
            userUpdated.last_name = userData.last_name;
            userUpdated.username = userData.username;
            userUpdated.phone = userData.phone;
            userUpdated.coins = userData.coins;
            context.SaveChanges();

            TempData["Success"] = "Successfully updated user information";
            return RedirectToAction("Index");
        }

        // GET: admin/user/tick?userId=int
        [HttpGet]
        public ActionResult Tick(int? userId)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            if (userId == null) return RedirectToAction("Index");
            user userHasTick = context.users.FirstOrDefault(user => user.id == userId);
            if (userHasTick == null) return RedirectToAction("Index");
            userHasTick.blue_tick = !userHasTick.blue_tick;
            context.SaveChanges();
            if (userHasTick.blue_tick)
            {
                TempData["Success"] = "Successfully set blue tick for user";
            } else
            {
                TempData["Success"] = "Successfully blue tick recall";
            }
            return RedirectToAction("Index");
        }

        // GET: admin/user/role?userId=int
        [HttpGet]
        public ActionResult Role(int? userId)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            if (userId == null) return RedirectToAction("Index");
            user userChangeRole = context.users.FirstOrDefault(user => user.id == userId);
            if (userChangeRole == null && UserLogin.GetUserLogin.role != UserLogin.AdminRole) return RedirectToAction("Index");
            ViewBag.userId = userChangeRole.id;

            return View();
        }

        // PUT: admin/user/changerole
        [HttpPut]
        public ActionResult ChangeRole(int? id, int role)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            if (id == null) return RedirectToAction("Index");
            user userChangeRole = context.users.FirstOrDefault(user => user.id == id);
            if (userChangeRole == null && UserLogin.GetUserLogin.role != UserLogin.AdminRole) return RedirectToAction("Index");
            if (role == 0 || role == 5 || role == 10)
            {
                userChangeRole.role = role;
            }
            context.SaveChanges();
            if (userChangeRole.id == UserLogin.GetUserLogin.id)
            {
                UserLogin.Update(userChangeRole);
            }
            TempData["Success"] = "Successfully change role";

            return RedirectToAction("Index");
        }

        // GET: admin/user/delete?userId=int
        [HttpGet]
        public ActionResult Delete(int? userId)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            if (userId == null) return RedirectToAction("Index");
            user userDelete = context.users.FirstOrDefault(user => user.id == userId);
            if (userDelete == null) return RedirectToAction("Index");

            // Remove relationship
            context.blogs.RemoveRange(context.blogs.Where(record => record.user_id == userId));
            context.comments.RemoveRange(context.comments.Where(record => record.user_id == userId));
            context.user_like_blogs.RemoveRange(context.user_like_blogs.Where(record => record.user_id == userId));
            context.user_like_comments.RemoveRange(context.user_like_comments.Where(record => record.user_id == userId));
            context.user_item_collections.RemoveRange(context.user_item_collections.Where(record => record.user_id == userId));

            String uploadFolderPath = Server.MapPath("~/Uploads");
            String oldFileName = userDelete.avatar;
            // Xóa file avt đi
            if (System.IO.File.Exists(uploadFolderPath + '/' + oldFileName) && !oldFileName.Equals("default-avatar.jpg"))
            {
                System.IO.File.Delete(uploadFolderPath + '/' + oldFileName);
            }
            context.users.Remove(userDelete);
            context.SaveChanges();
            TempData["Success"] = "Successfully delete user";
            return RedirectToAction("Index");
        }
    }
}