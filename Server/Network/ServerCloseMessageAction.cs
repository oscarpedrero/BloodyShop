using ProjectM.Network;
using BloodyShop.Network.Messages;
using Wetstone.API;
using BloodyShop.Server.DB;
using VRising.GameData;
using ProjectM;
using BloodyShop.Utils;

namespace BloodyShop.Server.Network
{
    public class ServerCloseMessageAction
    {

        public static void Received(FromCharacter fromCharacter, CloseSerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            //Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] CloseSerializedMessage {user.CharacterName}");

            CloseShop();

        }

        public static void CloseShop()
        {
            ConfigDB.ShopEnabled = false;
            ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($" {FontColorChat.White($" {ConfigDB.StoreName} ")} just closed"));
            var usersOnline = GameData.Users.Online;
            var msg = new CloseSerializedMessage();
            foreach (var user in usersOnline)
            {
                Send(user.Internals.User, msg);
            }
        }

        public static void Send(User fromCharacter, CloseSerializedMessage msg)
        {

            VNetwork.SendToClient(fromCharacter, msg);
            //Plugin.Logger.LogInfo($"[SERVER] [SEND] CloseSerializedMessage {fromCharacter.CharacterName}");

        }

    }
}
