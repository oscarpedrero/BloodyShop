using HarmonyLib;
using BloodyShop.Client;
using BloodyShop.Client.Patch;
using BloodyShop.Server;
using BloodyShop.Server.Network;
using BloodyShop.Client.UI;
using BloodyShop.Server.Patch;
using BloodyShop.Server.Systems;
using VampireCommandFramework;
using BloodyShop.Server.Commands;

namespace BloodyShop
{
    public class BloodyShop
    {
        public static void serverInitMod(Harmony _harmony)
        {
            _harmony.PatchAll(typeof(ServerEvents));
            ServerEvents.OnDeath += DropSystem.ServerEvents_OnDeath;
            ServerEvents.OnVampireDowned += DropSystem.ServerEvents_OnVampireDowned;
            ServerMod.CreateFilesConfig();
            CommandRegistry.RegisterCommandType(typeof(ShopCommands));
        }

        public static void clientInitMod(Harmony _harmony)
        {

            _harmony.PatchAll(typeof(ClientEvents));
            UIManager.Initialize();
            KeyBinds.Initialize();
            
            ClientEvents.OnClientConnected += ClientMod.ClientEvents_OnClientUserConnected;
            ClientEvents.OnClientDisconected += ClientMod.ClientEvents_OnClientUserDisconnected;
            KeyBinds.OnKeyPressed += KeyBindPressed.OnKeyPressedOpenPanel;

        }

        public static void onServerGameInitialized()
        {
            ServerMod.SetConfigMod();
        }

        public static void onClientGameInitialized()
        {
            NetworkMessages.RegisterMessage();
        }

        public static void serverUnloadMod()
        {
            ServerEvents.OnDeath -= DropSystem.ServerEvents_OnDeath;
        }

        public static void clientUnloadMod()
        {
            ClientEvents.OnClientConnected -= ClientMod.ClientEvents_OnClientUserConnected;
            ClientEvents.OnClientDisconected -= ClientMod.ClientEvents_OnClientUserDisconnected;
            KeyBinds.OnKeyPressed -= KeyBindPressed.OnKeyPressedOpenPanel;
        }

        public static void onServerGameDataOnInitialize()
        {
            NetworkMessages.RegisterMessage();
            ServerMod.LoadConfigToDB();
            ServerMod.LoadUserCoinsPerDayToDB();
        }

        public static void onClientGameDataOnInitialize()
        {
            ClientMod.ClientEvents_OnGameDataInitialized();
        }

    }
}
