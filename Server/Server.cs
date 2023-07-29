using BepInEx;
using BepInEx.Configuration;
using BloodyShop.DB;
using BloodyShop.Server.DB;
using BloodyShop.Server.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BloodyShop.Server
{
    public class ServerMod
    {

        public static ConfigEntry<bool> ShopEnabled;
        public static ConfigEntry<int> CoinGUID;

        public static readonly string ConfigPath = Path.Combine(Paths.ConfigPath, "BloodyShop");

        public static readonly string DropSystemPath = Path.Combine(ConfigPath, "DropSystem");

        public static string ProductListFile = Path.Combine(ConfigPath, "products_list.json");

        public static string UserCoinsPerDayFile = Path.Combine(ConfigPath, "user_coins_per_day.json");

        public static void CreateFilesConfig()
        {

            if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);

            if (!File.Exists(ProductListFile)) File.WriteAllText(ProductListFile, "");

            if (!Directory.Exists(DropSystemPath)) Directory.CreateDirectory(DropSystemPath);

            if (!File.Exists(UserCoinsPerDayFile)) File.WriteAllText(UserCoinsPerDayFile, "");
            
        }

        public static void LoadConfigToDB()
        {
            if (!LoadDataFromFiles.loadProductList())
            {
                Plugin.Logger.LogError($"Error loading ProductList");
            }
        }

        public static void LoadUserCoinsPerDayToDB()
        {
            if (!LoadDataFromFiles.loadUserCoinsPerDay())
            {
                Plugin.Logger.LogError($"Error loading loadUserCoinsPerDay");
            }
        }

        public static void SetConfigMod()
        {

            ConfigDB.ShopEnabled = Plugin.ShopEnabled.Value;
            ConfigDB.AnnounceAddRemovePublic = Plugin.AnnounceAddRemovePublic.Value;
            ConfigDB.AnnounceBuyPublic = Plugin.AnnounceBuyPublic.Value;

            var _storeName = Plugin.StoreName.Value;


            if (_storeName != string.Empty)
            {
                ConfigDB.StoreName = _storeName;
            }

            var _coinGUID = Plugin.CoinGUID.Value;
            var _coinName = Plugin.CoinName.Value;
            ShareDB.setCoinGUID(_coinGUID);
            ShareDB.setCoinName(_coinName);

            ConfigDB.DropEnabled = Plugin.DropEnabled.Value;

            ConfigDB.DropNpcPercentage = Plugin.DropNpcPercentage.Value;
            ConfigDB.IncrementPercentageDropEveryTenLevelsNpc = Plugin.IncrementPercentageDropEveryTenLevelsNpc.Value;
            ConfigDB.DropdNpcCoinsMin = Plugin.DropdNpcCoinsMin.Value;
            ConfigDB.DropNpcCoinsMax = Plugin.DropNpcCoinsMax.Value;
            ConfigDB.MaxCoinsPerDayPerPlayerNpc = Plugin.MaxCoinsPerDayPerPlayerNpc.Value;

            ConfigDB.DropdVBloodPercentage = Plugin.DropdVBloodPercentage.Value;
            ConfigDB.IncrementPercentageDropEveryTenLevelsVBlood = Plugin.IncrementPercentageDropEveryTenLevelsVBlood.Value;
            ConfigDB.DropVBloodCoinsMin = Plugin.DropVBloodCoinsMin.Value;
            ConfigDB.DropVBloodCoinsMax = Plugin.DropVBloodCoinsMax.Value;
            ConfigDB.MaxCoinsPerDayPerPlayerVBlood = Plugin.MaxCoinsPerDayPerPlayerVBlood.Value;

            ConfigDB.DropPvpPercentage = Plugin.DropPvpPercentage.Value;
            ConfigDB.IncrementPercentageDropEveryTenLevelsPvp = Plugin.IncrementPercentageDropEveryTenLevelsPvp.Value;
            ConfigDB.DropPvpCoinsMin = Plugin.DropPvpCoinsMin.Value;
            ConfigDB.DropPvpCoinsMax = Plugin.DropPvpCoinsMax.Value;
            ConfigDB.MaxCoinsPerDayPerPlayerPvp = Plugin.MaxCoinsPerDayPerPlayerPvp.Value;
        }
    }
}
