using BloodyShop.Client.DB;
using BloodyShop.Client.UI;
using BloodyShop.DB;
using BloodyShop.DB.Models;
using BloodyShop.Network.Messages;
using System;
using System.Collections.Generic;
using System.Text.Json;
using VRising.GameData;
using Wetstone.API;

namespace BloodyShop.Client.Network
{
    public class ClientConfigMessageAction
    {

        public static void Received(ConfigSerializedMessage msg)
        {
            Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] ConfigSerializedMessage {msg.ItemsJson} - {msg.CoinGUID} - {msg.ShopName} - {msg.ShopOpen} - {msg.ShopOpen}");

            var productList = JsonSerializer.Deserialize<List<ItemShopModel>>(msg.ItemsJson);
            ItemsDB.setProductList(productList);
            
            ShareDB.setCoinGUID(Int32.Parse(msg.CoinGUID));
            
            ClientDB.shopName = msg.ShopName;
            
            ClientDB.prefix = "!" + ClientDB.shopName.ToLower().Replace(" ", "");

            ClientDB.userModel = GameData.Users.GetCurrentUser();

            if (ClientDB.userModel.IsAdmin)
            {
                if (ItemsDB._normalizedItemNameCache.Count == 0)
                {
                    ItemsDB.generateCacheItems();
                }
            }

            if (msg.ShopOpen == "1")
            {
                ClientDB.shopOpen = true;

            }
            else
            {
                ClientDB.shopOpen = false;
            }

            ClientMod.UIInit = true;
            UIManager.CreateAllPanels();

        }

        public static void Send(ConfigSerializedMessage msg = null)
        {
            Plugin.Logger.LogInfo($"[CLIENT] [SEND] ConfigSerializedMessage");
            if (msg == null)
            {
                msg = new ConfigSerializedMessage();
                msg.ItemsJson = "list";
            }

            VNetwork.SendToServer(msg);

        }
    }
}
