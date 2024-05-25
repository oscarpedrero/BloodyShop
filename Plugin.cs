using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using BepInEx.Logging;
using Bloody.Core.GameData.v1;
using HarmonyLib;
using Unity.Entities;
using UnityEngine;
using Bloodstone.API;
using System;
using VampireCommandFramework;
using BloodyShop.Client.Patch;
using BloodyShop.Client;
using ProjectM.Network;
using Stunlock.Localization;
using Bloody.Core;
using Bloody.Core.API.v1;

namespace BloodyShop
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    [BepInDependency("gg.deca.Bloodstone")]
    [BepInDependency("trodi.Bloody.Core")]
    [Reloadable]
    public class Plugin : BasePlugin, IRunOnInitialized
    {

        internal static Plugin Instance;
        internal static string Name = MyPluginInfo.PLUGIN_NAME;
        internal static string Guid = MyPluginInfo.PLUGIN_GUID;
        internal static string Version = MyPluginInfo.PLUGIN_VERSION;

        public static Bloody.Core.Helper.v1.Logger Logger;
        private Harmony _harmony;

        public static ConfigEntry<bool> ShopEnabled;
        public static ConfigEntry<bool> AnnounceAddRemovePublic;
        public static ConfigEntry<bool> AnnounceBuyPublic;
        public static ConfigEntry<string> StoreName;

        public static SystemsCore SystemsCore;


        // Client Config
        public static ConfigEntry<bool> Sounds;

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

            if (!VWorld.IsServer)
            {
                return;
            }


            Instance = this;

            Logger = new(Log);
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            EventsHandlerSystem.OnInitialize += GameDataOnInitialize;
            EventsHandlerSystem.OnDestroy += GameDataOnDestroy;

            if (VWorld.IsServer)
            {
                InitConfigServer();
                BloodyShop.serverInitMod(_harmony);
                //ServerEvents.OnGameDataInitialized += GameDataOnInitialize;
            }
            else
            {
                InitConfigClient();
                BloodyShop.clientInitMod(_harmony);
                //ClientEvents.OnGameDataInitialized += GameDataOnInitialize;
                //ClientEvents.OnGameDataDestroyed += GameDataOnDestroy;
            }

            // Plugin startup logic
            Log.LogInfo($"BloodyShop is loaded!");
            
            

        }

        public override bool Unload()
        {

            if (VWorld.IsServer)
            {
                Config.Clear();
                BloodyShop.serverUnloadMod();
                CommandRegistry.UnregisterAssembly();
            }
            else
            {
                BloodyShop.clientUnloadMod();
            }

            _harmony.UnpatchSelf();
            
            EventsHandlerSystem.OnDestroy -= GameDataOnDestroy;
            EventsHandlerSystem.OnInitialize -= GameDataOnInitialize;
            return true;
        }

        private static void GameDataOnInitialize(World world)
        {
            SystemsCore = Core.SystemsCore;

            if (VWorld.IsServer)
            {
                BloodyShop.onServerGameDataOnInitialize();
            }
            else
            {
                try
                {
                    BloodyShop.onClientGameDataOnInitialize();
                }
                catch (Exception error)
                {
                    Logger.LogError($"Error GameDataOnInitialize {error.Message}");
                }
            }
        }

        private static void GameDataOnDestroy()
        {
            //Logger.LogInfo("GameDataOnDestroy");
        }

        private void InitConfigClient()
        {
            Sounds = Config.Bind("Client", "enabled", true, "Enable Sounds");
        }

        private void InitConfigServer()
        {
            ShopEnabled = Config.Bind("ConfigShop", "enabled", true, "Enable Shop");
            StoreName = Config.Bind("ConfigShop", "name", "Bloody Shop", "Store's name");
            AnnounceAddRemovePublic = Config.Bind("ConfigShop", "announceAddRemovePublic", true, "Public announcement when an item is added or removed from the store");
            AnnounceBuyPublic = Config.Bind("ConfigShop", "announceBuyPublic", true, "Public announcement when someone buys an item from the store");

        }

        public void OnGameInitialized()
        {

            //Logger.LogInfo("OnGameInitialized");
            if (VWorld.IsServer)
            {
                BloodyShop.onServerGameInitialized();
            }
            else
            {
                BloodyShop.onClientGameInitialized();
            }

        }
    }
}
