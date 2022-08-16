using BloodyShop.DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BloodyShop.Server.DB
{
    public class ConfigDB
    {
        private static bool ShopEnabled = true;

        private static string StoreName = "BloodyShop";

        public static bool getShopEnabled()
        {
            return ShopEnabled;
        }

        public static void setShopEnabled(bool value)
        {
            ShopEnabled = value;
        }

        public static string getStoreName()
        {
            return StoreName;
        }

        public static void setStoreName(string value)
        {
            StoreName = value;
        }

    }
}
