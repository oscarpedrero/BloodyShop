using UnityEngine;

namespace BloodyShop.Client.DB.Models
{
    public class PrefabModel
    {
        public string PrefabName { get; set; }
        public string PrefabType { get; set; }
        public int? PrefabGUID { get; set; }
        public Sprite PrefabIcon { get; set; }

    }
}
