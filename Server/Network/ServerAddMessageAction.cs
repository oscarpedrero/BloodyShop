using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using System.Text.Json;
using Bloodstone.API;
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
using BloodyShop.DB.Models;
using static RootMotion.FinalIK.Grounding;

namespace BloodyShop.Server.Network
{
    public class ServerAddMessageAction
    {
        public static CurrencyModel currency { get; private set; }

        public static void Received(FromCharacter fromCharacter, AddSerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            // Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] AddSerializedMessage {user.CharacterName} {msg.Stock} {int.Parse(msg.Stock)}");
            

            if(!user.IsAdmin)
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("You do not have permissions for this action"));

            var prefabGUID = int.Parse(msg.PrefabGUID);
            var currencyGUID = int.Parse(msg.CurrencyGUID);
            var price = int.Parse(msg.Price);
            var stock = int.Parse(msg.Stock);
            var stack = int.Parse(msg.Stack);
            var name = msg.Name;

            if(stock <= 0)
            {
                stock = -1;
            }

            //Plugin.Logger.LogInfo($"shop add {prefabGUID} {price} {stock}");

            addItem(user, prefabGUID, currencyGUID, price, stock, name, stack);

        }

        private static void addItem(User user, int item, int currencyGUID, int price, int stock, string name, int stack)
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

                if (stock <= 0)
                {
                    stock = -1;
                }

                if (stack <= 0)
                {
                    stack = 1;
                }

                currency = ShareDB.getCurrency(currencyGUID);

                if (currency == null)
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Error loading currency type"));
                    return;
                }

                if (!ItemsDB.addProductList(item, price, stock, name, currency.guid, stack))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Invalid item type"));
                    return;
                }

                SaveDataToFiles.saveProductList();

                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, $"Added item {FontColorChat.White($"{stack}x {name} ({stock})")} to the store with a price of {FontColorChat.White($"{price} {currency?.name.ToString()}")}");

                if (!ConfigDB.ShopEnabled)
                {
                    return;
                }

                if (ConfigDB.AnnounceAddRemovePublic)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChat.Yellow($"{FontColorChat.White($"{stack}x {name} ({stock})")} have been added to the Store for {FontColorChat.White($"{price} {currency?.name.ToString()}")}"));
                }

                /*var usersOnline = GameData.Users.Online;
                foreach (var userOnline in usersOnline)
                {
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send((ProjectM.Network.User)userOnline.Internals.User, msg);
                }*/

                var userWithUI = UserUI.GetUsersWithUI();
                foreach (var userUI in userWithUI)
                {
                    var userValue = userUI.Value;
                    if(userValue.IsConnected)
                    {
                        var msg = ServerListMessageAction.createMsg();
                        ServerListMessageAction.Send(userValue, msg);
                    }
                    
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
