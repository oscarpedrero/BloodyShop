using ProjectM;
using UnityEngine;
using VRising.GameData.Models;

namespace BloodyShop.DB.Models
{
    public class PrefabModel
    {
        public string PrefabName { get; set; }
        public int PrefabPrice { get; set; }
        public int PrefabStock { get; set; }
        public string PrefabType { get; set; }
        public int PrefabGUID { get; set; }
        public Sprite PrefabIcon { get; set; }
        public ItemModel itemModel { get; set; }


        /*public PrefabGUID getPrefabGUID()
        {
            return new PrefabGUID(PrefabGUID);
        }*/

        public bool CheckStockAvailability(int numberofItemsBuy)
        {
            if (PrefabStock < 0) return true;

            if (PrefabStock >= numberofItemsBuy)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
