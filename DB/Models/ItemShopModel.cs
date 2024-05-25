using Bloody.Core.GameData.v1;
using ProjectM;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Bloody.Core.Models.v1;

namespace BloodyShop.DB.Models
{
    public class ItemShopModel
    {
        public int id { get; set; }
        public string name { get; set; }

        public int price { get; set; }

        public int stock { get; set; }
        public int stack { get; set; }
        public int currency { get; set; }
        public bool isBuff { get; set; }

    }
}
