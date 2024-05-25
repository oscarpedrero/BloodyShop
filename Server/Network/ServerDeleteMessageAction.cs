/*using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Bloodstone.API;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Commands;
using System;
using BloodyShop.DB.Models;
using BloodyShop.Utils;
using BloodyShop.Server.DB;
using Bloody.Core.GameData.v1;
using ProjectM;

namespace BloodyShop.Server.Network
{
    public class ServerDeleteMessageAction
    {

        public static void Received(FromCharacter fromCharacter, DeleteSerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            //Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] DeleteSerializedMessage {user.CharacterName} - {msg.Item}");

            if (!user.IsAdmin)
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("You do not have permissions for this action"));

            var itemID = Int32.Parse(msg.Item);

            removeItemFromShop(user, itemID);

        }

        private static void removeItemFromShop(User user, int index)
        {
            try
            {
                if (!ItemsDB.SearchItemByCommand(index, out PrefabModel itemShopModel))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red(FontColorChat.Yellow($"Item removed error.")));
                    return;
                }
                if (!ItemsDB.RemoveItemByCommand(index))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"Item {FontColorChat.White($"{itemShopModel.PrefabName}")} removed error."));
                    return;
                }

                SaveDataToFiles.saveProductList();
                LoadDataFromFiles.loadProductList();

                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Yellow($"Item {FontColorChat.White($"{itemShopModel.PrefabName}x {itemShopModel.PrefabName}")} removed successful."));

                var userWithUI = UserUI.GetUsersWithUI();
                foreach (var userUI in userWithUI)
                {
                    var userValue = userUI.Value;
                    if (userValue.IsConnected)
                    {
                        var msg = ServerListMessageAction.createMsg();
                        ServerListMessageAction.Send(userValue, msg);
                    }
                }

                if (ConfigDB.AnnounceAddRemovePublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"Item {FontColorChat.White($"{itemShopModel.PrefabName}x {itemShopModel.PrefabName}")} removed from the store."));
                }
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"Error: {error.Message}"));
            }
        }

        public static void Send(User fromCharacter, DeleteSerializedMessage msg)
        {
           return;
        }

    }
}
*/