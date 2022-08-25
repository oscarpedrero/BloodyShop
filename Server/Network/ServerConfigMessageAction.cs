using BloodyShop.DB;
using BloodyShop.Network.Messages;
using BloodyShop.Server.DB;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Wetstone.API;

namespace BloodyShop.Server.Network
{
    public class ServerConfigMessageAction
    {

        public static void Received(User fromCharacter, ConfigSerializedMessage msg)
        {

            msg = createMsg();

            Send(fromCharacter, msg);

            Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] ConfigSerializedMessage {fromCharacter.CharacterName}");

        }

        public static ConfigSerializedMessage createMsg()
        {

            var msg = new ConfigSerializedMessage();
            var productList = ItemsDB.getProductListForSaveJSON();
            var jsonOutPut = JsonSerializer.Serialize(productList);

            msg.ItemsJson = jsonOutPut;
            msg.CoinGUID = ShareDB.getCoinGUID().ToString();
            msg.ShopName = ConfigDB.getStoreName();

            if (ConfigDB.getShopEnabled())
            {
                msg.ShopOpen = "1";
            }
            else
            {
                msg.ShopOpen = "0";
            }

            return msg;
        }

        public static void Send(User fromCharacter, ConfigSerializedMessage msg)
        {
            VNetwork.SendToClient(fromCharacter, msg);
            Plugin.Logger.LogInfo($"[SERVER] [SEND] ConfigSerializedMessage {fromCharacter.CharacterName} - {msg.ItemsJson} - {msg.CoinGUID} - {msg.ShopName} - {msg.ShopOpen}");
        }
    }
}
