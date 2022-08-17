using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Wetstone.API;
using BloodyShop.Server.DB;

namespace BloodyShop.Server.Network
{
    public class ServerListMessageAction
    {

        public static void Received(User fromCharacter, ListSerializedMessage msg)
        {

            var productList = ItemsDB.GetProductList();
            var jsonOutPut = JsonSerializer.Serialize(productList);

            msg.ItemsJson = jsonOutPut;
            msg.CoinGUID = ShareDB.getCoinGUID().ToString();
            msg.ShopName = ConfigDB.getStoreName();
            msg.CharacterName = fromCharacter.CharacterName.ToString();

            Send(fromCharacter,msg);

            Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] ListSerializedMessage {fromCharacter.CharacterName} - {msg.ItemsJson}");

        }

        public static void Send(User fromCharacter, ListSerializedMessage msg)
        {
            VNetwork.SendToClient(fromCharacter, msg);
            Plugin.Logger.LogInfo($"[SERVER] [SEND] ListSerializedMessage {fromCharacter.CharacterName} - {msg.ItemsJson} - {msg.CoinGUID}");
        }

    }
}
