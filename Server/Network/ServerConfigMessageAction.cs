using BloodyShop.DB;
using BloodyShop.Network.Messages;
using BloodyShop.Server.DB;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using VRising.GameData;
using Wetstone.API;

namespace BloodyShop.Server.Network
{
    public class ServerConfigMessageAction
    {

        public static void Received(User fromCharacter, ConfigSerializedMessage msg)
        {

            msg = createMsg(fromCharacter);

            Send(fromCharacter, msg);

            //Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] ConfigSerializedMessage {fromCharacter.CharacterName}");

        }

        public static ConfigSerializedMessage createMsg(User fromCharacter)
        {

            var msg = new ConfigSerializedMessage();
            var productList = ItemsDB.getProductListForSaveJSON();
            var jsonOutPut = JsonSerializer.Serialize(productList);

            msg.ItemsJson = jsonOutPut;
            msg.CoinGUID = ShareDB.getCoinGUID().ToString();
            msg.ShopName = ConfigDB.StoreName;
            var userModel = GameData.Users.GetUserByCharacterName(fromCharacter.CharacterName.ToString());

            if (userModel.IsAdmin)
            {
                msg.isAdmin = "1";
            } else
            {
                msg.isAdmin = "0";
            }

            if (ConfigDB.ShopEnabled)
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
            //Plugin.Logger.LogInfo($"[SERVER] [SEND] ConfigSerializedMessage {fromCharacter.CharacterName} - {msg.ItemsJson} - {msg.CoinGUID} - {msg.ShopName} - {msg.ShopOpen}");
        }
    }
}
