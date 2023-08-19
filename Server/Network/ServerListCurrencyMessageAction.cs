using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Bloodstone.API;
using BloodyShop.Server.DB;
using System;

namespace BloodyShop.Server.Network
{
    public class ServerListCurrencyMessageAction
    {

        public static void Received(User fromCharacter, ListCurrencySerializedMessage msg)
        {

            msg = createMsg();

            Send(fromCharacter,msg);

            //Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] ListSerializedMessage {fromCharacter.CharacterName}");

        }

        public static ListCurrencySerializedMessage createMsg()
        {

            var msg = new ListCurrencySerializedMessage();
            var productList = ShareDB.getCurrencyList();
            var jsonOutPut = JsonSerializer.Serialize(productList);
            msg.CurrencyJson = jsonOutPut;

            return msg;
        }

        public static void Send(User fromCharacter, ListCurrencySerializedMessage msg)
        {
            VNetwork.SendToClient(fromCharacter, msg);
            //Plugin.Logger.LogInfo($"[SERVER] [SEND] ListSerializedMessage {fromCharacter.CharacterName} - {msg.ItemsJson}");
        }

    }
}
