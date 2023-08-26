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
            var drop = bool.Parse(msg.Drop);
            Plugin.Logger.LogWarning(msg.Drop);
            Plugin.Logger.LogWarning(drop);

            //Plugin.Logger.LogInfo($"shop add {prefabGUID} {price} {stock}");

            addCurrency(user,currencyGUID, name, drop);

        }

        private static void addCurrency(User user, int currencyGUID, string name, bool drop)
        {
            try
            {
                var prefabGUID = new PrefabGUID(currencyGUID);
                var currencyModel = GameData.Items.GetPrefabById(prefabGUID);

                if (currencyModel == null)
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Invalid currency type"));
                    sendMessageToAdmin();
                    return;
                }

                currency = ShareDB.getCurrency(currencyGUID);

                if (currency != null)
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red($"The currency {FontColorChat.White($"{name}")} already exists in the store."));
                    sendMessageToAdmin();
                    return;
                }

                if (!ShareDB.addCurrencyList(name,currencyGUID, drop))
                {
                    ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, FontColorChat.Red("Invalid currency type"));
                    sendMessageToAdmin();
                    return;
                }

                SaveDataToFiles.saveCurrenciesList();

                ServerChatUtils.SendSystemMessageToClient(VWorld.Server.EntityManager, user, $"Added currency {FontColorChat.White($"{name}")} to the store.");

                if (!ConfigDB.ShopEnabled)
                {
                    return;
                }

                sendMessageToAdmin();

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
        private static void sendMessageToAdmin()
        {
            var userWithUI = UserUI.GetUsersWithUI();
            foreach (var userUI in userWithUI)
            {
                var userValue = userUI.Value;
                if (userValue.IsConnected && userValue.IsAdmin)
                {
                    var msg = ServerListMessageAction.createMsg();
                    ServerListMessageAction.Send((ProjectM.Network.User)userValue, msg);
                }
            }
        }

    }
}
