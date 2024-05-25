using BloodyShop.DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BloodyShop.Server.DB
{
    public class ConfigDB
    {
        public static bool ShopEnabled { get; set; } = true;
        public static bool AnnounceAddRemovePublic { get; set; } = true;
        public static bool AnnounceBuyPublic { get; set; } = true;
        public static string StoreName { get; set; } = "Bloody Shop";
        
    }
}
