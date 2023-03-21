using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using NOSBlog.Auths;
using NOSBlog.Models;

namespace NOSBlog.Areas.Admin.Controllers
{
    public class ItemController : Controller
    {
        private NOSBlogEntities context = new NOSBlogEntities();

        // GET: /admin/item
        public ActionResult Index()
        {
            // if (!UserLogin.IsAdmin) return Redirect("/");
            List<item> listItem = context.items.ToList();
            ViewBag.items = listItem;
            return View();
        }

        // GET: Admin/Item/Create
        public ActionResult Create()
        {
            // if (!UserLogin.IsAdmin) return Redirect("/");
            return View();
        }

        // POST: /admin/item/store
        [HttpPost]
        public ActionResult Store(item itemData, HttpPostedFileBase imageFile)
        {
            // if (!UserLogin.IsAdmin) return Redirect("/");
            if (!ModelState.IsValid) return View("Create");
            if (itemData.reduce > itemData.price) return View("Create");

            item newItem = new item();
            if (imageFile != null && imageFile.ContentLength > 0 && imageFile.ContentLength <= 32000000)
            {
                String prefix = DateTime.Now.ToString("ddMMyyyyHHmmss-ms");
                String uploadFolderPath = Server.MapPath("~/Uploads/Items");

                String extName = Path.GetExtension(imageFile.FileName);
                Random random = new Random();
                int randomNumber = random.Next();
                String newImgFileName = prefix + "item-img" + randomNumber + extName;
                imageFile.SaveAs(uploadFolderPath + "/" + newImgFileName);

                newItem.image = newImgFileName;
            } else
            {
                return RedirectToAction("Create");
            }

            
            newItem.name = itemData.name;
            newItem.price = itemData.price;
            newItem.reduce = itemData.reduce;
            newItem.quantity = itemData.quantity;
            newItem.type = itemData.type;
            newItem.collection_points = itemData.collection_points;
            newItem.@lock = false;
            newItem.created_at = DateTime.Now;
            newItem.updated_at = DateTime.Now;

            context.items.Add(newItem);
            context.SaveChanges();

            return Redirect("/admin/item");
        }
    }
}