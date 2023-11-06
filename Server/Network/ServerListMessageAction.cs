using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Bloodstone.API;
using BloodyShop.Server.DB;
using System;

namespace BloodyShop.Server.Network
{
    public class ServerListMessageAction
    {

        public static void Received(User fromCharacter, ListSerializedMessage msg)
        {

            msg = createMsg();

            Send(fromCharacter,msg);

            //Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] ListSerializedMessage {fromCharacter.CharacterName}");

        }

        public static ListSerializedMessage createMsg()
        {

            var msg = new ListSerializedMessage();
            var productList = ItemsDB.getProductListForSaveJSON();
            var jsonOutPut = JsonSerializer.Serialize(productList);

            msg.ItemsJson = jsonOutPut;

            Plugin.Logger.LogInfo(jsonOutPut);

            var currencyList = ShareDB.getCurrencyList();
            jsonOutPut = JsonSerializer.Serialize(currencyList);

            msg.CurrencyJson = jsonOutPut;

            return msg;
        }

        public static void Send(User fromCharacter, ListSerializedMessage msg)
        {
            VNetwork.SendToClient(fromCharacter, msg);
            //Plugin.Logger.LogInfo($"[SERVER] [SEND] ListSerializedMessage {fromCharacter.CharacterName} - {msg.ItemsJson}");
        }

    }
}
