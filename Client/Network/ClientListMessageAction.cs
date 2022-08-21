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
    public class ClientListMessageAction
    {

        public static void Received(ListSerializedMessage msg)
        {
            Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] ListSerializedMessage {msg.ItemsJson} - {msg.CoinGUID} - {msg.ShopName} - {msg.ShopOpen} - {msg.ShopOpen}");

            var productList = JsonSerializer.Deserialize<List<ItemShopModel>>(msg.ItemsJson);
            ItemsDB.setProductList(productList);
            ShareDB.setCoinGUID(Int32.Parse(msg.CoinGUID));
            ClientDB.shopName = msg.ShopName;
            ClientDB.prefix = "!" + ClientDB.shopName.ToLower().Replace(" ", "");
            ClientDB.userModel = GameData.Users.GetCurrentUser();
            if(ClientDB.itemModels.Count == 0)
            {
                ClientDB.itemModels = GameData.Items.Prefabs;
            }
            

            if (msg.ShopOpen == "1")
            {
                ClientDB.shopOpen = true;

            } else
            {
                ClientDB.shopOpen = false;
            }

            if (!ClientMod.UIInit)
            {
                ClientMod.UIInit = true;
                UIManager.CreateMenuPanel();
            } else
            {
                if (ClientDB.shopOpen)
                {
                    UIManager.MenuPanel?.openShop();
                    UIManager.AdminMenuPanel?.openShop();
                } else
                {
                    UIManager.MenuPanel?.closeShop();
                    UIManager.AdminMenuPanel?.closeShop();
                }
                if (UIManager.ShopPanel?.productsListLayers.Count > 0)
                {
                    UIManager.ShopPanel?.RefreshData();
                }

                if (UIManager.DeleteItemPanel?.productsListLayers.Count > 0)
                {
                    UIManager.DeleteItemPanel?.RefreshData();
                }
                UIManager.ShopPanel?.CreateListProductsLayou();
                UIManager.DeleteItemPanel?.CreateListProductsLayou();
            }
            
        }

        public static void Send(ListSerializedMessage msg = null)
        {
            Plugin.Logger.LogInfo($"[CLIENT] [SEND] ListSerializedMessage");
            if (msg == null)
            {
                msg = new ListSerializedMessage();
                msg.ItemsJson = "list";
            }

            VNetwork.SendToServer(msg);
            
        }

    }
}
