using BloodyShop.DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BloodyShop.Server.DB
{
    public class ConfigDB
    {
        public static bool ShopEnabled = true;

        public static string StoreName = "BloodyShop";

        public static bool DropEnabled = true;

        public static int DropNpcPercentage { get; set; } = 0;
        public static int DropdNpcCoinsMin { get; set; } = 0;
        public static int DropNpcCoinsMax { get; set; } = 0;

        public static int DropdVBloodPercentage { get; set; } = 0;
        public static int DropVBloodCoinsMin { get; set; } = 0;
        public static int DropVBloodCoinsMax { get; set; } = 0;

        public static int DropPvpPercentage { get; set; } = 0;
        public static int DropPvpCoinsMin { get; set; } = 0;
        public static int DropPvpCoinsMax { get; set; } = 0;

    }
}
