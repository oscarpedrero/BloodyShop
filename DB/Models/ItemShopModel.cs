using VRising.GameData;
using ProjectM;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VRising.GameData.Models;

namespace BloodyShop.DB.Models
{
    public class ItemShopModel
    {
        public int id { get; set; }

        public int price { get; set; }

        public int amount { get; set; }


        public PrefabGUID getPrefabGUID()
        {
            return new PrefabGUID(id);
        }

        public string getItemName()
        {
            var item = GameData.Items.GetPrefabById(new PrefabGUID(id));

            return item?.Name.ToString();
        }

        public ItemModel getItem()
        {
            var item = GameData.Items.GetPrefabById(new PrefabGUID(id));

            return item;
        }

        public Sprite getIcon()
        {
            var item = GameData.Items.GetPrefabById(new PrefabGUID(id));
            return item?.ManagedGameData.ManagedItemData?.Icon;
        }

        public string getItemType()
        {
            var item = GameData.Items.GetPrefabById(new PrefabGUID(id));
            var intemTypeReturn = "";
            switch (item?.ItemType.ToString())
            {
                case "Stackable":
                    intemTypeReturn = "Ingredient";
                    break;
                case "ItemBuilding":
                    intemTypeReturn = "Other";
                    break;
                default:
                    intemTypeReturn = item?.ItemType.ToString();
                    break;
            }

            return intemTypeReturn;



        }

        public string getPrefabName()
        {
            var item = GameData.Items.GetPrefabById(new PrefabGUID(id));
            return item?.PrefabName.ToString();
        }

        public bool CheckAmountAvailability(int numberofItemsBuy)
        {
            if(amount >= numberofItemsBuy)
            {
                return true;
            } else
            {
                return false;
            }
        }

    }
}
