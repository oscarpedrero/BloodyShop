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
        public static bool gameDataInit { get; set; } = false;
        public static List<ItemModel> allItemsGame { get; set; }
        public static string[] allItemsTypeGame { get; set; }
        public static string shopName { get; set; }
        public static bool shopOpen { get; set; } = true;
        public static string prefix { get; set; }
        public static UserModel userModel { get; set; }

        public static void generateTypesOfItems()
        {
            
            var allTypesItems =allItemsGame.GroupBy(i => i.ItemType)
                .Select(grp => grp.FirstOrDefault())    
                .OrderBy(i => i.ItemType).ToArray();
            
            allItemsTypeGame = new string[allTypesItems.Count() - 1];
            allItemsTypeGame[0] = "Select Type";
            var index = 1;
            foreach (var item in allTypesItems)
            {
                if (item.ItemType.ToString() == "VBloodEssence" || item.ItemType.ToString() == "Memory")
                {
                    continue;
                }
                allItemsTypeGame[index] = item.ItemType.ToString();
                index++;
            }
            

        }
        public static string[] getAllTypes()
        {
            return allItemsTypeGame;
        }

        public static string[] getAllItemsByType(string type)
        {
            var allItemsByType = from item in allItemsGame
                                where item.ItemType.ToString() == type
                                orderby item.Name.ToString() ascending
                                select item;

            string[] allItemsByTypeArray = new string[allItemsByType.Count()+1];
            allItemsByTypeArray[0] = "Select Itemm";
            var index = 1;
            foreach (var item in allItemsByType)
            {
                allItemsByTypeArray[index] = item.Name + " | " + item.Internals.PrefabGUID?.GuidHash;
                index++;
            }

            return allItemsByTypeArray;
        }

    }
}