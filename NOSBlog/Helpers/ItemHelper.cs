using NOSBlog.Auths;
using NOSBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NOSBlog.Helpers
{
    public static class ItemHelper
    {

        public static int MinusOne(int num1, int num2)
        {
            return num1 - num2;
        }

        public static bool IsBought(int itemId)
        {
            NOSBlogEntities context = new NOSBlogEntities();
            if (!UserLogin.IsUserLogin) return false;
            int userId = UserLogin.GetUserLogin.id;

            user buyer = context.users.FirstOrDefault(user => user.id == userId);
            item itemSell = context.items.FirstOrDefault(item => item.id == itemId);

            if (buyer == null || itemSell == null) return false;

            user_item_collections rdCheck = context.user_item_collections.FirstOrDefault(x => x.user_id == buyer.id && x.item_id == itemSell.id);
            if (rdCheck != null) return true;
            return false;
        }
    }
}