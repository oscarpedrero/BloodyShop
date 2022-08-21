using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRising.GameData;
using VRising.GameData.Models;

namespace BloodyShop.Client.DB
{
    public class ClientDB
    {
        public static bool serverHaveMod { get; set; } = false;
        public static bool gameDataInit { get; set; } = false;
        public static string shopName { get; set; }
        public static bool shopOpen { get; set; } = true;
        public static string prefix { get; set; }
        public static UserModel userModel { get; set; }

        public static List<ItemModel> itemModels { get; set; } = new List<ItemModel>();

        public static List<ItemModel> searchItemByName(string text)
        {
            var itemsFind = from item in itemModels
                            where item.Name != null && item.Name.ToString().ToLower().Contains(text.ToString().ToLower())
                            orderby item.ItemType.ToString() ascending, item.Name.ToString() ascending
                            select item;

            return itemsFind.ToList();

        }

    }
}