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
        public static ConfigEntry<int> CurrencyGUID;

        public static readonly string ConfigPath = Path.Combine(Paths.ConfigPath, "BloodyShop");

        public static readonly string DropSystemPath = Path.Combine(ConfigPath, "DropSystem");

        public static string ProductListFile = Path.Combine(ConfigPath, "products_list.json");

        public static string CurrencyListFile = Path.Combine(ConfigPath, "currency_list.json");

        public static string UserCurrenciesPerDayFile = Path.Combine(ConfigPath, "user_currencies_per_day.json");

        public static void CreateFilesConfig()
        {

            if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);

            if (!File.Exists(ProductListFile)) File.WriteAllText(ProductListFile, "[]");
            if (!File.Exists(CurrencyListFile)) File.WriteAllText(CurrencyListFile, "[{\"id\":1,\"name\":\"Silver Coin\",\"guid\":-949672483}]");

            if (!Directory.Exists(DropSystemPath)) Directory.CreateDirectory(DropSystemPath);

            if (!File.Exists(UserCurrenciesPerDayFile)) File.WriteAllText(UserCurrenciesPerDayFile, "");
            
        }

        public static void LoadConfigToDB()
        {
            if (!LoadDataFromFiles.loadProductList())
            {
                Plugin.Logger.LogError($"Error loading ProductList");
            }
        }

        public static void LoadCurrenciesToDB()
        {
            if (!LoadDataFromFiles.loadCurrencies())
            {
                Plugin.Logger.LogError($"Error loading CurrenciesList");
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

            
        }
    }
}
