/*using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Bloodstone.API;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Commands;
using BloodyShop.Server.DB;
using ProjectM;
using BloodyShop.Utils;
using Bloody.Core.GameData.v1;

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
            var msg = new OpenSerializedMessage();
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

        public static void Send(User fromCharacter, OpenSerializedMessage msg)
        {
            VNetwork.SendToClient(fromCharacter, msg);
            //Plugin.Logger.LogInfo($"[SERVER] [SEND] CloseSerializedMessage {fromCharacter.CharacterName}");
        }

    }
}
*/