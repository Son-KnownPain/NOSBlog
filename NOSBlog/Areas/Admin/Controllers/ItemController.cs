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
            if (!UserLogin.IsAdmin) return Redirect("/");
            List<item> listItem = context.items.ToList();
            ViewBag.items = listItem;
            return View();
        }

        // GET: Admin/Item/Create
        public ActionResult Create()
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            return View();
        }

        // POST: /admin/item/store
        [HttpPost]
        public ActionResult Store(item itemData, HttpPostedFileBase imageFile)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
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

            TempData["Success"] = "Successfully add new item";
            return Redirect("/admin/item");
        }

        // GET /Admin/Item/Delete
        [HttpGet]
        public ActionResult Delete(int? itemId)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            if (itemId == null) return RedirectToAction("Index");
            item itemToDelete = context.items.FirstOrDefault(item => item.id == itemId);
            if (itemToDelete == null) return RedirectToAction("Index");

            // Xóa điểm người dùng theo điểm lúc mua
            List<int> listUserIDBought = context.user_item_collections
                .Where(rd => rd.item_id == itemId).Select(rd => rd.user_id).ToList();
            foreach (int userID in listUserIDBought)
            {
                user userUpdatePoints = context.users.FirstOrDefault(user => user.id == userID);
                if (userUpdatePoints != null)
                {
                    int itemPoints = context.user_item_collections.Where(rd => rd.item_id == itemId && rd.user_id == userID).Select(rd => rd.collection_points).FirstOrDefault();

                    userUpdatePoints.collection_points -= itemPoints;
                }
            }
            // Remove relationship
            context.user_item_collections.RemoveRange(context.user_item_collections.Where(item => item.item_id == itemId));

            context.items.Remove(itemToDelete);

            // Remove item image
            String uploadFolderPath = Server.MapPath("~/Uploads/Items");
            String oldFileName = itemToDelete.image;
            // Xóa file image đi
            if (System.IO.File.Exists(uploadFolderPath + '/' + oldFileName))
            {
                System.IO.File.Delete(uploadFolderPath + '/' + oldFileName);
            }

            context.SaveChanges();

            TempData["Success"] = "Successfully delete item";
            return RedirectToAction("Index");
        }

        // GET /Admin/Item/Lock
        [HttpGet]
        public ActionResult Lock(int? itemId)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            if (itemId == null) return RedirectToAction("Index");
            item itemLock = context.items.FirstOrDefault(item => item.id == itemId);
            if (itemLock == null) return RedirectToAction("Index");
            itemLock.@lock = !itemLock.@lock;
            itemLock.updated_at = DateTime.Now;

            context.SaveChanges();

            TempData["Success"] = "Successfully lock or unlock item";
            return RedirectToAction("Index");
        }

        // GET /Admin/Item/Edit
        [HttpGet]
        public ActionResult Edit(int? itemId)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            if (itemId == null) return RedirectToAction("Index");
            item itemToEdit = context.items.FirstOrDefault(item => item.id == itemId);
            if (itemToEdit == null) return RedirectToAction("Index");

            return View(itemToEdit);
        }

        // PUT /Admin/Item/Update
        [HttpPut]
        public ActionResult Update(item itemData, HttpPostedFileBase imageFile)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            if (!ModelState.IsValid) return View("Edit");

            item itemUpdate = context.items.FirstOrDefault(item => item.id == itemData.id);
            if (itemUpdate != null)
            {
                itemUpdate.name = itemData.name;
                itemUpdate.price = itemData.price;
                itemUpdate.reduce = itemData.reduce;
                itemUpdate.quantity = itemData.quantity;
                itemUpdate.type = itemData.type;
                itemUpdate.collection_points = itemData.collection_points;

                if (imageFile != null)
                {
                    // Remove item image
                    String uploadFolderPath = Server.MapPath("~/Uploads/Items");
                    String oldFileName = itemUpdate.image;
                    // Xóa file image đi
                    if (System.IO.File.Exists(uploadFolderPath + '/' + oldFileName))
                    {
                        System.IO.File.Delete(uploadFolderPath + '/' + oldFileName);
                    }

                    String prefix = DateTime.Now.ToString("ddMMyyyyHHmmss-ms");

                    String extName = Path.GetExtension(imageFile.FileName);
                    Random random = new Random();
                    int randomNumber = random.Next();
                    String newImgFileName = prefix + "item-img" + randomNumber + extName;
                    imageFile.SaveAs(uploadFolderPath + "/" + newImgFileName);

                    itemUpdate.image = newImgFileName;
                }

                context.SaveChanges();
                TempData["Success"] = "Successfully update item";
            }

            return RedirectToAction("Index");
        }
    }
}