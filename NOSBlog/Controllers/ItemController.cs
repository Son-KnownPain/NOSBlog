using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NOSBlog.Auths;
using NOSBlog.Models;

namespace NOSBlog.Controllers
{
    public class ItemController : Controller
    {
        private NOSBlogEntities context = new NOSBlogEntities();

        // GET: Item
        public ActionResult Index()
        {
            List<item> items = context.items.ToList();
            ViewBag.items = items;
            return View();
        }

        // GET: Item/Buy?itemId=int
        public ActionResult Buy(int? itemId)
        {
            if (itemId == null || !UserLogin.IsUserLogin) return RedirectToAction("Index");
            int userId = UserLogin.GetUserLogin.id;

            user buyer = context.users.FirstOrDefault(user => user.id == userId);
            item itemSell = context.items.FirstOrDefault(item => item.id == itemId);

            if (buyer == null || itemSell == null) return RedirectToAction("Index");

            user_item_collections rdCheck = context.user_item_collections.FirstOrDefault(x => x.user_id == buyer.id && x.item_id == itemSell.id);
            if (rdCheck != null)
            {
                TempData["Error"] = "You bought that item";
                return RedirectToAction("Index");
            }

            if (buyer.coins < (itemSell.price - itemSell.reduce))
            {
                TempData["Error"] = "Your coins is not enough to buy";
                return RedirectToAction("Index");
            }
            if (itemSell.quantity <= 0)
            {
                TempData["Error"] = "Sold out";
                return RedirectToAction("Index");
            }
            if (itemSell.@lock)
            {
                TempData["Error"] = "This item is no longer for sale";
                return RedirectToAction("Index");
            }

            // Valid -> handle
            user_item_collections newRd = new user_item_collections();
            newRd.user_id = buyer.id;
            newRd.item_id = itemSell.id;
            newRd.price = itemSell.price - itemSell.reduce;
            newRd.collection_points = itemSell.collection_points;

            buyer.coins -= (itemSell.price - itemSell.reduce);
            buyer.collection_points += itemSell.collection_points;

            itemSell.quantity -= 1;

            context.user_item_collections.Add(newRd);
            context.SaveChanges();

            TempData["Success"] = "Successfully buy item";
            return RedirectToAction("Index");
        }
    }
}