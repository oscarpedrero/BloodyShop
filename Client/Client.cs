using BloodyShop.Client.Network;
using BloodyShop.Client.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Entities;

namespace BloodyShop.Client
{
    public class ClientMod
    {

        public static bool UIInit { get; set; }

        public static void ClientEvents_OnGameDataInitialized(World world)
        {
            UIInit = false;
            Plugin.Logger.LogInfo("Init send message to server");
            
        }
    }
}
