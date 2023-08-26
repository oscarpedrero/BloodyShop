using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRising.GameData.Models;

namespace BloodyShop.DB.Models
{
    [Serializable]
    public class CurrencyModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int guid { get; set; }
        public bool drop { get; set; } = true;

    }
}
