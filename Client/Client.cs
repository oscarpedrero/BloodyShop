using BloodyShop.Client.DB;
using BloodyShop.Client.DB.Models;
using BloodyShop.Client.Network;
using BloodyShop.Client.UI;
using ProjectM;
using System;
using System.Collections.Generic;
using VRising.GameData;
using VRising.GameData.Models;
using Wetstone.API;

namespace BloodyShop.Client
{
    public class ClientMod
    {
        
        public static bool UIInit { get; set; }

        public static void ClientEvents_OnGameDataInitialized()
        {
            Plugin.Logger.LogInfo("ClientEvents_OnGameDataInitialized");
            UIInit = false;
        }

        public static void ClientEvents_OnClientUserConnected()
        {
            Plugin.Logger.LogInfo("ClientEvents_OnClientUserConnected");
            ClientListMessageAction.Send();
        }

        public static void ClientEvents_OnClientUserDisconnected()
        {
            Plugin.Logger.LogInfo("ClientEvents_OnClientUserDisconnected");
            UIInit = false;
            try
            {
                if (ClientDB.userModel.IsAdmin)
                {
                    UIManager.AdminMenuPanel?.Destroy();
                } else
                {
                    UIManager.MenuPanel?.Destroy();
                }
                if (UIManager.ActiveShopPanel || UIManager.ActiveDeleteItemPanel || UIManager.ActiveAddItemPanel)
                {
                    UIManager.ActiveShopPanel = false;
                    try
                    {
                        UIManager.ShopPanel?.Destroy();
                    }
                    catch { }

                    if (ClientDB.userModel.IsAdmin)
                    {
                        UIManager.ActiveDeleteItemPanel = false;
                        UIManager.DeleteItemPanel?.Destroy();
                    }
                    if (ClientDB.userModel.IsAdmin)
                    {
                        UIManager.ActiveAddItemPanel = false;
                        UIManager.AddItemPanel?.Destroy();
                    }
                }
            } catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }

        }
    }
}
