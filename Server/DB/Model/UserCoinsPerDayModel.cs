using System;
using System.Collections.Generic;
using System.Text;

namespace BloodyShop.Server.DB.Model
{
    public class UserCurrenciesPerDayModel
    {
        public string CharacterName { get; set; }
        public string date { get; set; }
        public int AmountNpc { get; set; } = 0;
        public int AmountVBlood { get; set; } = 0;
        public int AmountPvp { get; set; } = 0;
    }
}
