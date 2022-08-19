using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRising.GameData.Models;

namespace BloodyShop.Client.DB
{
    public class ClientDB
    {
        public static bool serverHaveMod { get; set; } = false;
        public static List<ItemModel> allItemsGame { get; set; }
        public static string shopName { get; set; }
        public static bool shopOpen { get; set; } = true;
        public static string prefix { get; set; }
        public static UserModel userModel { get; set; }

        public static string[] getAllTypes()
        {
            var allTypesItems = from item in allItemsGame
                                group item by item.ItemType into itemGroup
                                orderby itemGroup.ToString() ascending
                                select itemGroup;

            string[] allTypesItemsArray = new string[allTypesItems.Count()-2];
            var index = 0;
            foreach (var itemGroup in allTypesItems)
            {
                if(itemGroup.Key.ToString() == "VBloodEssence" || itemGroup.Key.ToString() == "Memory" )
                {
                    continue;
                }
                allTypesItemsArray[index] = itemGroup.Key.ToString();
                index++;
            }

            return allTypesItemsArray;
        }

        public static string[] getAllItemsByType(string type)
        {
            var allItemsByType = from item in allItemsGame
                                where item.ItemType.ToString() == type
                                orderby item.ItemType.ToString() ascending
                                select item;

            string[] allItemsByTypeArray = new string[allItemsByType.Count()];
            var index = 0;
            foreach (var item in allItemsByType)
            {
                allItemsByTypeArray[index] = item.Name + " | " + item.Internals.PrefabGUID?.GuidHash;
                index++;
            }

            return allItemsByTypeArray;
        }

    }
}