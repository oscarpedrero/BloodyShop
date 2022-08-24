using HarmonyLib;
using BloodyShop.Client;
using BloodyShop.Client.Patch;
using BloodyShop.Server;
using BloodyShop.Server.Hooks;
using BloodyShop.Server.Network;
using BloodyShop.Client.UI;
using BloodyShop.Client.DB;
using VRising.GameData;

namespace BloodyShop
{
    public class BloodyShop
    {
        public static void serverInitMod(Harmony _harmony)
        {
            _harmony.PatchAll(typeof(ChatMessageSystem_Patch));
            ServerMod.CreateFilesConfig();
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

        public static void onServerGameInitialized(bool ShopEnabled, int CoinGUID, string StoreName)
        {
            ServerMod.SetConfigMod(ShopEnabled, CoinGUID, StoreName);
        }

        public static void onClientGameInitialized()
        {
            NetworkMessages.RegisterMessage();
        }

        public static void serverUnloadMod()
        {
            
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
        }

        public static void onClientGameDataOnInitialize()
        {
            ClientMod.ClientEvents_OnGameDataInitialized();
        }

    }
}
