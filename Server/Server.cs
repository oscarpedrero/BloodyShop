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

        public static void SetConfigMod(bool ShopEnabled, int CoinGUID, string StoreName)
        {
            ConfigDB.setShopEnabled(ShopEnabled);
            if(StoreName != string.Empty)
            {
                ConfigDB.setStoreName(StoreName);
                ChatSystem.SetPrefix(StoreName);
            }
            ShareDB.setCoinGUID(CoinGUID);
        }
    }
}
