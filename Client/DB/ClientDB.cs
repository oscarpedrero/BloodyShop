using System;
using System.Collections.Generic;
using System.Text;
using VRising.GameData.Models;

namespace BloodyShop.Client.DB
{
    public class ClientDB
    {
        public static string[] allItemsGame { get; set; }
        public static string shopName { get; set; }
        public static string prefix { get; set; }
        public static UserModel userModel { get; set; }

    }
}