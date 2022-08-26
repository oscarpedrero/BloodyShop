using BloodyShop.Client.DB;
using BloodyShop.Client.Network;
using BloodyShop.Client.UI;
using BloodyShop.DB;
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
            ClientConfigMessageAction.Send();
        }

        public static void ClientEvents_OnClientUserConnected()
        {
            Plugin.Logger.LogInfo("ClientEvents_OnClientUserConnected");
        }

        public static void ClientEvents_OnClientUserDisconnected()
        {
            Plugin.Logger.LogInfo("ClientEvents_OnClientUserDisconnected");
            UIInit = false;
            UIManager.DestroyAllPanels();
        }
    }
}
