using BloodyShop.Client.DB;
//using BloodyShop.Client.Network;
using BloodyShop.Client.UI;
using BloodyShop.DB;
using ProjectM;
using System;
using System.Collections.Generic;
using Bloody.Core.GameData.v1;
using Bloody.Core.Models.v1;
using Bloodstone.API;
//using BloodyShop.Server.Network;
using BloodyShop.AutoAnnouncer.Timers;
using BloodyShop.AutoAnnouncer;

namespace BloodyShop.Client
{
    public class ClientMod
    {
        private static AutoLoadUiTimer _autoLoadUiTimer;

        public static bool UIInit { get; set; }

        public static void ClientEvents_OnGameDataInitialized()
        {
            //Plugin.Logger.LogInfo("ClientEvents_OnGameDataInitialized");
            UIInit = false;
            _autoLoadUiTimer = new AutoLoadUiTimer();
            StartAutoUI();
            //ClientConfigMessageAction.Send();
        }

        public static void ClientEvents_OnClientUserConnected()
        {
            //Plugin.Logger.LogInfo("ClientEvents_OnClientUserConnected");
        }

        public static void ClientEvents_OnClientUserDisconnected()
        {
            //Plugin.Logger.LogInfo("ClientEvents_OnClientUserDisconnected");
            UIInit = false;
            UIManager.DestroyAllPanels();
        }

        private static void StartAutoUI()
        {
            _autoLoadUiTimer.Start(
                world =>
                {
                    //Plugin.Logger.LogInfo("Starting UI...");
                    AutoUIFunction.OnTimedAutoUI();
                },
                input =>
                {
                    if (input is not int secondAutoUIr)
                    {
                        Plugin.Logger.LogError("Starting UI timer delay function parameter is not a valid integer");
                        return TimeSpan.MaxValue;
                    }

                    var seconds = 30;
                    //Plugin.Logger.LogInfo($"Next Starting UI will start in {seconds} seconds.");
                    return TimeSpan.FromSeconds(seconds);
                });
        }

        public static void StopAutoUI()
        {
            _autoLoadUiTimer?.Stop();
        }
    }
}
