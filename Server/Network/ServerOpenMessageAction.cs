using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Wetstone.API;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Commands;
using BloodyShop.Server.DB;
using ProjectM;
using BloodyShop.Utils;
using VRising.GameData;

namespace BloodyShop.Server.Network
{
    public class ServerOpenMessageAction
    {

        public static void Received(FromCharacter fromCharacter, OpenSerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            //Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] OpenSerializedMessage {user.CharacterName}");

            OpenShop();

        }

        public static void OpenShop()
        {

            ConfigDB.ShopEnabled = true;
            ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($" {FontColorChat.White($" {ConfigDB.StoreName} ")} just opened"));
            var usersOnline = GameData.Users.Online;
            var msg = new OpenSerializedMessage();
            foreach (var user in usersOnline)
            {
                Send(user.Internals.User, msg);
            }

        }

        public static void Send(User fromCharacter, OpenSerializedMessage msg)
        {
            VNetwork.SendToClient(fromCharacter, msg);
            //Plugin.Logger.LogInfo($"[SERVER] [SEND] CloseSerializedMessage {fromCharacter.CharacterName}");
        }

    }
}
