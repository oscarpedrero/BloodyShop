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

        public static string ProductListFile = Path.Combine(ConfigPath, "products_list.json");

        public static void CreateFilesConfig()
        {

            if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);

            if (!File.Exists(ProductListFile)) File.WriteAllText(ProductListFile, "");
            
        }

        public static void LoadConfigToDB()
        {
            if (!LoadDataFromFiles.loadProductList())
            {
                Plugin.Logger.LogError($"Error loading ProductList");
            }
        }

        public static void SetConfigMod()
        {
            var _shopEnabled = Plugin.ShopEnabled.Value;
            ConfigDB.ShopEnabled = _shopEnabled;

            var _storeName = Plugin.StoreName.Value;


            if (_storeName != string.Empty)
            {
                ConfigDB.StoreName = _storeName;
                ChatSystem.SetPrefix(_storeName);
            }

            var _coinGUID = Plugin.CoinGUID.Value;
            ShareDB.setCoinGUID(_coinGUID);

            ConfigDB.DropEnabled = Plugin.DropEnabled.Value;

            ConfigDB.DropNpcPercentage = Plugin.DropNpcPercentage.Value;
            ConfigDB.DropdNpcCoinsMin = Plugin.DropdNpcCoinsMin.Value;
            ConfigDB.DropNpcCoinsMax = Plugin.DropNpcCoinsMax.Value;

            ConfigDB.DropdVBloodPercentage = Plugin.DropdVBloodPercentage.Value;
            ConfigDB.DropVBloodCoinsMin = Plugin.DropVBloodCoinsMin.Value;
            ConfigDB.DropVBloodCoinsMax = Plugin.DropVBloodCoinsMax.Value;

            ConfigDB.DropPvpPercentage = Plugin.DropPvpPercentage.Value;
            ConfigDB.DropPvpCoinsMin = Plugin.DropPvpCoinsMin.Value;
            ConfigDB.DropPvpCoinsMax = Plugin.DropPvpCoinsMax.Value;
        }
    }
}
