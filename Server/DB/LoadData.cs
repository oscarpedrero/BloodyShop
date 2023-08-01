using BloodyShop.DB;
using BloodyShop.DB.Models;
using BloodyShop.Server.DB.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BloodyShop.Server.DB
{
    internal class LoadDataFromFiles
    {

        public static bool loadProductList()
        {
            try
            {
                string json = File.ReadAllText(ServerMod.ProductListFile);
                var productList = JsonSerializer.Deserialize<List<ItemShopModel>>(json);
                //Plugin.Logger.LogInfo($"Total product List FROM JSON {productList.Count}");
                return ItemsDB.setProductList(productList);
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }

        }

        public static bool loadCurrencies()
        {
            try
            {
                string json = File.ReadAllText(ServerMod.CurrencyListFile);

                var currenciesList = JsonSerializer.Deserialize<List<CurrencyModel>>(json);
                return ShareDB.setCurrencyList(currenciesList);
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }

        }

        public static bool loadUserCurrenciesPerDay()
        {
            try
            {
                string json = File.ReadAllText(ServerMod.UserCurrenciesPerDayFile);
                var usersCurrenciesPerDaList = JsonSerializer.Deserialize<List<UserCurrenciesPerDayModel>>(json);
                //Plugin.Logger.LogInfo($"Total usersCurrenciesPerDay List FROM JSON {usersCurrenciesPerDaList.Count}");
                return ConfigDB.setUsersCurrenciesPerDay(usersCurrenciesPerDaList);
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }

        }
    }
}
