using ProjectM.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using Bloodstone.API;
using BloodyShop.Server.Systems;
using System;
using ProjectM;
using VRising.GameData;
using BloodyShop.Server.DB;
using BloodyShop.Utils;
using BloodyShop.DB.Models;

namespace BloodyShop.Server.Network
{
    public class ServerAddCurrencyMessageAction
    {
        public static CurrencyModel currency { get; private set; }

        public static void Received(FromCharacter fromCharacter, AddCurrencySerializedMessage msg)
        {
            var user = VWorld.Server.EntityManager.GetComponentData<User>(fromCharacter.User);

            //Plugin.Logger.LogInfo($"[SERVER] [RECEIVED] AddSerializedMessage {user.CharacterName}");
            

            if(!user.IsAdmin)
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("You do not have permissions for this action"));

            var currencyGUID = int.Parse(msg.CurrencyGUID);
            var name = msg.Name;

            //Plugin.Logger.LogInfo($"shop add {prefabGUID} {price} {stock}");

            addCurrency(user,currencyGUID, name);

        }

        private static void addCurrency(User user, int currencyGUID, string name)
        {
            try
            {
                var prefabGUID = new PrefabGUID(currencyGUID);
                var currencyModel = GameData.Items.GetPrefabById(prefabGUID);

                if (currencyModel == null)
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Invalid currency type"));
                    return;
                }

                currency = ShareDB.getCurrency(currencyGUID);

                if (currency != null)
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"The currency {FontColorChat.White($"{name}")} already exists in the store."));
                    return;
                }

                if (!ShareDB.addCurrencyList(name,currencyGUID))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Invalid currency type"));
                    return;
                }

                SaveDataToFiles.saveCurrenciesList();

                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, $"Added currency {FontColorChat.White($"{name}")} to the store.");

                if (!ConfigDB.ShopEnabled)
                {
                    return;
                }

                /*
                var usersOnline = GameData.Users.Online;
                foreach (var userOnline in usersOnline)
                {
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send((ProjectM.Network.User)userOnline.Internals.User, msg);
                }
                */

                var userWithUI = UserUI.GetUsersWithUI();
                foreach (var userUI in userWithUI)
                {
                    var userValue = userUI.Value;
                    if(userValue.IsConnected && userValue.IsAdmin)
                    {
                        // TODO SEND CUrrencys
                        var msg = ServerListMessageAction.createMsg();
                        ServerListMessageAction.Send(userValue, msg);
                    }
                }

                return;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError(error.Message);
                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Error saving the currency in the store "));
            }
        }

        public static void Send(User fromCharacter, AddSerializedMessage msg)
        {
           return;
        }

    }
}
