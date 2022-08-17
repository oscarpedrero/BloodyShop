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
            var productList = JsonSerializer.Deserialize<List<ItemShopModel>>(msg.ItemsJson);
            ItemsDB.setProductList(productList);
            ShareDB.setCoinGUID(Int32.Parse(msg.CoinGUID));
            ClientDB.shopName = msg.ShopName;
            ClientDB.prefix = "!" + ClientDB.shopName.ToLower().Replace(" ", "");
            ClientDB.userModel = GameData.Users.GetUserByCharacterName(msg.CharacterName);

            if (!ClientMod.UIInit)
            {
                ClientMod.UIInit = true;
                UIManager.createPanel();
            }
            Plugin.Logger.LogInfo($"[CLIENT] [RECEIVED] ListSerializedMessage {msg.ItemsJson} - {msg.CoinGUID}");
        }

        public static void Send(ListSerializedMessage msg = null)
        {

            if(msg == null)
            {
                msg = new ListSerializedMessage();
                msg.ItemsJson = "list";
            }

            VNetwork.SendToServer(msg);
            Plugin.Logger.LogInfo($"[CLIENT] [SEND] ListSerializedMessage  - {msg.ItemsJson}");
        }

    }
}
