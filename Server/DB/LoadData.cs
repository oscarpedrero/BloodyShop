using BloodyShop.DB;
using BloodyShop.DB.Models;
using BloodyShop.Server.DB.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
                Plugin.Logger.LogInfo($"Total product List FROM JSON {productList.Count}");
                return ItemsDB.setProductList(productList);
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }

        }

        public static bool loadUserCoinsPerDay()
        {
            try
            {
                string json = File.ReadAllText(ServerMod.UserCoinsPerDayFile);
                var usersCoinsPerDaList = JsonSerializer.Deserialize<List<UserCoinsPerDayModel>>(json);
                Plugin.Logger.LogInfo($"Total usersCoinsPerDay List FROM JSON {usersCoinsPerDaList.Count}");
                return ConfigDB.setUsersCoinsPerDay(usersCoinsPerDaList);
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }

        }
    }
}
