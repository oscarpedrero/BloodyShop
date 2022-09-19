using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Wetstone.API;
using BloodyShop.Server.Systems;
using BloodyShop.Server.Commands;
using System;
using ProjectM;
using VRising.GameData;
using VRising.GameData.Models;
using BloodyShop.Server.DB;
using BloodyShop.Utils;
using Unity.Entities;
using VampireCommandFramework.Breadstone;
using Unity.Collections;

namespace BloodyShop.Server.Network
{
    public class ServerAddMessageAction
    {

        public static void Received(FromCharacter fromCharacter, AddSerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            //Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] AddSerializedMessage {user.CharacterName}");
            

            var prefabGUID = int.Parse(msg.PrefabGUID);
            var price = int.Parse(msg.Price);
            var stock = int.Parse(msg.Stock);

            if(stock <= 0)
            {
                stock = -1;
            }

            //Plugin.Logger.LogInfo($"shop add {prefabGUID} {price} {stock}");

            addItem(user, prefabGUID, price, stock);

        }

        private static void addItem(User user, int item, int price, int stock)
        {
            try
            {
                var prefabGUID = new PrefabGUID(item);
                var itemModel = GameData.Items.GetPrefabById(prefabGUID);

                if (itemModel == null)
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Invalid item type"));
                    return;
                }

                if (!ShareDB.getCoin(out ItemModel coin))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Error loading currency type"));
                    return;
                }

                if (!ItemsDB.addProductList(item, price, stock))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Invalid item type"));
                    return;
                }

                SaveDataToFiles.saveProductList();

                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, $"Added item {FontColorChat.White($"{ itemModel?.Name.ToString()} ({stock})")} to the store with a price of {FontColorChat.White($"{price} {coin?.Name.ToString()}")}");

                if (!ConfigDB.ShopEnabled)
                {
                    return;
                }

                if (ConfigDB.AnnounceAddRemovePublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"{FontColorChat.White($"{itemModel?.Name.ToString()} ({stock})")} have been added to the Store for {FontColorChat.White($"{price} {coin?.Name.ToString()}")}"));
                }

                var usersOnline = GameData.Users.Online;
                foreach (var userOnline in usersOnline)
                {
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send(userOnline.Internals.User, msg);
                }
                return;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError(error.Message);
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Error saving the item in the store "));
            }
        }

        public static void Send(User fromCharacter, AddSerializedMessage msg)
        {
           return;
        }

    }
}
