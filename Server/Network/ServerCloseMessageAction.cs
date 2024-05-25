/*using ProjectM.Network;
using BloodyShop.Network.Messages;
using Bloodstone.API;
using BloodyShop.Server.DB;
using Bloody.Core.GameData.v1;
using ProjectM;
using BloodyShop.Utils;
using static VCF.Core.Basics.RoleCommands;
using BloodyShop.Server.Systems;

namespace BloodyShop.Server.Network
{
    public class ServerCloseMessageAction
    {

        public static void Received(FromCharacter fromCharacter, CloseSerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<ProjectM.Network.User>(fromCharacter.User);

            //Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] CloseSerializedMessage {user.CharacterName}");

            CloseShop();

        }

        public static void CloseShop()
        {
            ConfigDB.ShopEnabled = false;
            ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($" {FontColorChat.White($" {ConfigDB.StoreName} ")} just closed"));
            var msg = new CloseSerializedMessage();
            var userWithUI = UserUI.GetUsersWithUI();
            foreach (var userUI in userWithUI)
            {
                var userValue = userUI.Value;
                if (userValue.IsConnected)
                {
                    Send(userValue, msg);
                }
            }
        }

        public static void Send(ProjectM.Network.User fromCharacter, CloseSerializedMessage msg)
        {

            VNetwork.SendToClient(fromCharacter, msg);
            //Plugin.Logger.LogInfo($"[SERVER] [SEND] CloseSerializedMessage {fromCharacter.CharacterName}");

        }

    }
}
*/