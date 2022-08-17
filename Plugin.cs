using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using VRising.GameData;
using HarmonyLib;
using BloodyShop.Server.Network;
using Unity.Entities;
using UnityEngine;
using Wetstone.API;
using System.Linq;
using BloodyShop.Client.UI.Panels;

namespace BloodyShop
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("xyz.molenzwiebel.wetstone")]
    public class Plugin : BasePlugin, IRunOnInitialized
    {

        internal static Plugin Instance;
        internal static string Name = PluginInfo.PLUGIN_NAME;
        internal static string Guid = PluginInfo.PLUGIN_GUID;
        internal static string Version = PluginInfo.PLUGIN_VERSION;

        public static ManualLogSource Logger;
        private Harmony _harmony;

        public static ConfigEntry<bool> ShopEnabled;
        public static ConfigEntry<int> CoinGUID;
        public static ConfigEntry<string> StoreName;

        private static World _serverWorld;
        private static World _clientWorld;

        public static World Server
        {
            get
            {
                if (_serverWorld != null) return _serverWorld;

                _serverWorld = GetWorld("Server")
                    ?? throw new System.Exception("There is no Server world (yet). Did you install a server mod on the client?");
                return _serverWorld;
            }
        }
        public static World Client
        {
            get
            {
                if (_clientWorld != null) return _clientWorld;

                _clientWorld = GetWorld("Client")
                    ?? throw new System.Exception("There is no Client world (yet). Did you install a client mod on the server?");
                return _clientWorld;
            }
        }

        public static bool IsServer => Application.productName == "VRisingServer";
        public static bool IsClient => Application.productName == "VRisingClient";

        private static World GetWorld(string name)
        {
            foreach (var world in World.s_AllWorlds)
            {
                if (world.Name == name)
                {
                    return world;
                }
            }

            return null;
        }

        public override void Load()
        {
            Instance = this;

            Logger = Log;
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            

            if (VWorld.IsServer)
            {
                InitConfigServer();
                BloodyShop.serverInitMod(_harmony);

            } else
            {
                BloodyShop.clientInitMod(_harmony);
            }

            // Plugin startup logic
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            GameData.OnInitialize += GameDataOnInitialize;

        }

        public override bool Unload()
        {

            if (VWorld.IsServer)
            {
                Config.Clear();
                BloodyShop.serverUnloadMod();
            }
            else
            {
                BloodyShop.clientUnloadMod();
            }
            
            _harmony.UnpatchSelf();
            GameData.OnInitialize -= GameDataOnInitialize;
            return true;
        }

        private static void GameDataOnInitialize(World world)
        {
            Logger.LogWarning("GameData Init");
            if (VWorld.IsServer)
            {
                BloodyShop.onServerGameDataOnInitialize();
            }
            else
            {
                BloodyShop.onClientGameDataOnInitialize();
            }
        }

        private void InitConfigServer()
        {
            ShopEnabled = Config.Bind("Shop", "enabled", true, "Enable Shop");
            StoreName = Config.Bind("Shop", "name", "Bloody Shop", "Store's name. This name will also serve as a prefix for the command, that is, if you put Black Market, for example, the system will parse the name, remove space and pass it to lowercase, so the command will be !blackmarket");
            CoinGUID = Config.Bind("Config", "coinGUID", -949672483, "Item that will be used as currency within the service, by default they are silver coins, if you want to change the item you must include the GUID of said object that you can get from https://gaming.tools/v-rising/items");
        }

        public void OnGameInitialized()
        {

            Logger.LogInfo("OnGameInitialized");
            if (VWorld.IsServer)
            {
                BloodyShop.onServerGameInitialized(ShopEnabled.Value, CoinGUID.Value, StoreName.Value);
            }
            else
            {
                BloodyShop.onClientGameInitialized();
            }

        }
    }
}
