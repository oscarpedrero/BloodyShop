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
    public class ServerDeleteMessageAction
    {

        public static void Received(FromCharacter fromCharacter, DeleteSerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] DeleteSerializedMessage {user.CharacterName} - {msg.Item}");

            var itemID = msg.Item;

            var prefix = ChatSystem.GetPrefix();

            var vchatEvent = new VChatEvent(fromCharacter.User, fromCharacter.Character,$"{prefix} rm {itemID} 1", new ChatMessageType(), user);

            string[] args = new string[] {itemID};

            var ctx = new Context(prefix, vchatEvent, args);

            Remove.removeItemFromShop(ctx);

            

        }

        public static void Send(User fromCharacter, DeleteSerializedMessage msg)
        {
           return;
        }

    }
}
