using BloodyShop.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace BloodyShop.Server.DB
{
    internal class SaveDataToFiles
    {

        public static bool saveProductList()
        {
            try
            {
                var productList = ItemsDB.getProductListForSaveJSON();
                var jsonOutPut = JsonSerializer.Serialize(productList);
                File.WriteAllText(ServerMod.ProductListFile, jsonOutPut);
                
                return true;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }

        }
    }
}
