using VRising.GameData;
using ProjectM;
using System;
using System.Collections.Generic;
using System.Text;

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
