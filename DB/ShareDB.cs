using VRising.GameData;
using VRising.GameData.Models;
using ProjectM;
using BloodyShop.DB.Models;

namespace BloodyShop.DB
{
    public class ShareDB
    {
        public static int CoinGUID = 0;

        public static string CoinName = "";

        public static int getCoinGUID()
        {
            return CoinGUID;
        }

        public static int getCoinName()
        {
            return CoinGUID;
        }

        public static bool getCoin( out PrefabModel coin)
        {
            coin = new PrefabModel();

            if (CoinGUID != 0)
            {
                coin.PrefabName = CoinName;
                coin.itemModel = GameData.Items.GetPrefabById(new PrefabGUID(CoinGUID));
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

        public static void setCoinName(string name)
        {
            
            CoinName = name;
            
        }
    }
}
