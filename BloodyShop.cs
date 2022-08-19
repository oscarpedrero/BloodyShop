using HarmonyLib;
using BloodyShop.Client;
using BloodyShop.Client.Patch;
using BloodyShop.Server;
using BloodyShop.Server.Hooks;
using BloodyShop.Server.Network;
using System;
using BloodyShop.Client.UI;
using VRising.GameData;
using System.Linq;
using BloodyShop.Client.UI.Panels;
using BloodyShop.Client.DB;
using System.Collections.Generic;
using VRising.GameData.Models;

namespace BloodyShop
{
    public class BloodyShop
    {
        public static void serverInitMod(Harmony _harmony)
        {
            _harmony.PatchAll(typeof(ChatMessageSystem_Patch));
            ServerMod.CreateFilesConfig();
            ServerMod.LoadConfigToDB();
        }

        public static void clientInitMod(Harmony _harmony)
        {
            UIManager.Initialize();
            KeyBinds.Initialize();
            
            ClientEvents.OnGameDataInitialized += ClientMod.ClientEvents_OnGameDataInitialized;
            KeyBinds.OnKeyPressed += KeyBindPressed.OnKeyPressedOpenPanel;
        }

        public static void onServerGameInitialized(bool ShopEnabled, int CoinGUID, string StoreName)
        {
            ServerMod.SetConfigMod(ShopEnabled, CoinGUID, StoreName);
        }

        public static void onClientGameInitialized()
        {
            
        }

        public static void serverUnloadMod()
        {
            
        }

        public static void clientUnloadMod()
        {
            KeyBinds.OnKeyPressed -= KeyBindPressed.OnKeyPressedOpenPanel;
        }

        public static void onServerGameDataOnInitialize()
        {
            NetworkMessages.RegisterMessage();
        }

        public static void onClientGameDataOnInitialize()
        {
            NetworkMessages.RegisterMessage();
            ClientDB.allItemsGame = GameData.Items.Prefabs;
        }

    }
}
