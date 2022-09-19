using VRising.GameData;
using VRising.GameData.Models;
using ProjectM;

namespace BloodyShop.DB
{
    public class ShareDB
    {
        public static int CoinGUID = 0;

        public static int getCoinGUID()
        {
            return CoinGUID;
        }

        public static bool getCoin( out ItemModel coin)
        {
            coin = null;
            if (CoinGUID != 0)
            {
                coin = GameData.Items.GetPrefabById(new PrefabGUID(CoinGUID));
                return true;
            } else
            {
                return false;
            }
            
        }

        public static void setCoinGUID(int value)
        {
            
            CoinGUID = value;
            
        }
    }
}
