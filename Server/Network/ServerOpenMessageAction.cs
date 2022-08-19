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
    public class ServerOpenMessageAction
    {

        public static void Received(FromCharacter fromCharacter, OpenSerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] OpenSerializedMessage {user.CharacterName}");

            var prefix = ChatSystem.GetPrefix();

            var vchatEvent = new VChatEvent(fromCharacter.User, fromCharacter.Character,$"{prefix} open", new ChatMessageType(), user);

            string[] args = new string[0];

            var ctx = new Context(prefix, vchatEvent, args);

            Open.OpenShop(ctx);


        }

        public static void Send(User fromCharacter, OpenSerializedMessage msg)
        {
            VNetwork.SendToClient(fromCharacter, msg);
            Plugin.Logger.LogInfo($"[SERVER] [SEND] CloseSerializedMessage {fromCharacter.CharacterName}");
        }

    }
}
