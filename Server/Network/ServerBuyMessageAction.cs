using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Wetstone.API;
using BloodyShop.Server.Utils;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Commands;

namespace BloodyShop.Server.Network
{
    public class ServerBuyMessageAction
    {

        public static void Received(FromCharacter fromCharacter, BuySerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] BuySerializedMessage {user.CharacterName} - {msg.ItemIndex}");

            var buyID = msg.ItemIndex;

            var prefix = ChatSystem.GetPrefix();

            var vchatEvent = new VChatEvent(fromCharacter.User, fromCharacter.Character,$"{prefix} buy {buyID} 1", new ChatMessageType(), user);

            string[] args = new string[] { buyID , "1"};

            var ctx = new Context(prefix, vchatEvent, args);

            Buy.BuyItem(ctx);

            

        }

        public static void Send(User fromCharacter, BuySerializedMessage msg)
        {
           return;
        }

    }
}
