using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Wetstone.API;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Commands;
using System;
using BloodyShop.DB.Models;
using BloodyShop.Utils;
using BloodyShop.Server.DB;
using VRising.GameData;
using ProjectM;

namespace BloodyShop.Server.Network
{
    public class ServerDeleteMessageAction
    {

        public static void Received(FromCharacter fromCharacter, DeleteSerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            //Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] DeleteSerializedMessage {user.CharacterName} - {msg.Item}");

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

                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Yellow($"Item {FontColorChat.White($"{itemShopModel.PrefabName}")} removed successful."));

                var usersOnline = GameData.Users.Online;
                foreach (var userOnline in usersOnline)
                {
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send(userOnline.Internals.User, msg);
                }

                if (ConfigDB.AnnounceAddRemovePublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"Item {FontColorChat.White($"{itemShopModel.PrefabName}")} removed successful."));
                }
            }
            catch (Exception error)
            {
                Plugin.Logger.LogInfo($"Error: {error.Message}");
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"Error: {error.Message}"));
            }
        }

        public static void Send(User fromCharacter, DeleteSerializedMessage msg)
        {
           return;
        }

    }
}
