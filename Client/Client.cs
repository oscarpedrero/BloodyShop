using BloodyShop.Client.DB;
using BloodyShop.Client.Network;
using BloodyShop.Client.UI;
using System;
using Unity.Entities;
using VRising.GameData;

namespace BloodyShop.Client
{
    public class ClientMod
    {

        public static bool UIInit { get; set; }

        public static void ClientEvents_OnGameDataInitialized(World world)
        {

            UIInit = false;
            
        }

        public static void ClientEvents_OnClientUserConnected()
        {
            Plugin.Logger.LogInfo("OnClientUserConnected");
            ClientDB.userModel = GameData.Users.GetCurrentUser();
            ClientDB.allItemsGame = GameData.Items.Prefabs;
            ClientDB.generateTypesOfItems();
            ClientListMessageAction.Send();
        }

        public static void ClientEvents_OnClientUserDisconnected()
        {
            Plugin.Logger.LogInfo("OnClientUserDisconnected");
            UIInit = false;
            Plugin.Instance.unloadGameData();
            Plugin.Instance.LoadGameData();
            try
            {
                UIManager.MenuPanel?.Destroy();
                UIManager.ShopPanel?.Destroy();
                UIManager.AdminPanel?.Destroy();
            } catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }

        }
    }
}
