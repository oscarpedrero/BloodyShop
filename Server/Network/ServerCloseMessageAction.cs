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
    public class ServerCloseMessageAction
    {

        public static void Received(FromCharacter fromCharacter, CloseSerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] CloseSerializedMessage {user.CharacterName}");

            var prefix = ChatSystem.GetPrefix();

            var vchatEvent = new VChatEvent(fromCharacter.User, fromCharacter.Character,$"{prefix} close", new ChatMessageType(), user);

            string[] args = new string[0];

            var ctx = new Context(prefix, vchatEvent, args);

            Close.CloseShop(ctx);

        }

        public static void Send(User fromCharacter, CloseSerializedMessage msg)
        {
           return;
        }

    }
}
