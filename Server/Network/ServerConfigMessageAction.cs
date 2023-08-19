using BloodyShop.DB;
using BloodyShop.Network.Messages;
using BloodyShop.Server.DB;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using VRising.GameData;
using Bloodstone.API;
using BloodyShop.Server.Systems;

namespace BloodyShop.Server.Network
{
    public class ServerConfigMessageAction
    {

        public static void Received(User fromCharacter, ConfigSerializedMessage msg)
        {
            //Plugin.Logger.LogError($"[SERVER] [RECEIVED] ConfigSerializedMessage {fromCharacter.CharacterName}");
            msg = createMsg(fromCharacter);
            UserUI.RegisterUserWithUI(fromCharacter);
            Send(fromCharacter, msg);
        }

        public static ConfigSerializedMessage createMsg(User fromCharacter)
        {

            var msg = new ConfigSerializedMessage();
            var productList = ItemsDB.getProductListForSaveJSON();
            var jsonOutItemsPut = JsonSerializer.Serialize(productList);

            msg.ItemsJson = jsonOutItemsPut;

            var currencies = ShareDB.getCurrencyList();
            var jsonOuCurrenciestPut = JsonSerializer.Serialize(currencies);
            msg.CurrenciesJson = jsonOuCurrenciestPut;

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
            //Plugin.Logger.LogInfo($"[SERVER] [SEND] ConfigSerializedMessage {fromCharacter.CharacterName} - {msg.ItemsJson} - {msg.CurrencyGUID} - {msg.ShopName} - {msg.ShopOpen}");
        }
    }
}
