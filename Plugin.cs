using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using BepInEx.Logging;
using VRising.GameData;
using HarmonyLib;
using Unity.Entities;
using UnityEngine;
using Bloodstone.API;
using System;
using VampireCommandFramework;
using BloodyShop.Client.Patch;
using BloodyShop.Client;
using BloodyShop.Server.Patch;
using ProjectM.Network;
using Stunlock.Localization;

namespace BloodyShop
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    [BepInDependency("gg.deca.Bloodstone")]

    public class Plugin : BasePlugin, IRunOnInitialized
    {

        internal static Plugin Instance;
        internal static string Name = MyPluginInfo.PLUGIN_NAME;
        internal static string Guid = MyPluginInfo.PLUGIN_GUID;
        internal static string Version = MyPluginInfo.PLUGIN_VERSION;

        public static ManualLogSource Logger;
        private Harmony _harmony;

        public static ConfigEntry<bool> ShopEnabled;
        public static ConfigEntry<bool> AnnounceAddRemovePublic;
        public static ConfigEntry<bool> AnnounceBuyPublic;
        public static ConfigEntry<string> StoreName;


        /// 
        /// DROP SYSTEM
        /// 

        public static ConfigEntry<bool> DropEnabled;

        // NPC CONFIG DROP
        public static ConfigEntry<int> DropNpcPercentage;
        public static ConfigEntry<int> IncrementPercentageDropEveryTenLevelsNpc;
        public static ConfigEntry<int> DropdNpcCurrenciesMin;
        public static ConfigEntry<int> DropNpcCurrenciesMax;
        public static ConfigEntry<int> MaxCurrenciesPerDayPerPlayerNpc;

        // VBLOOD CONFIG DROP
        public static ConfigEntry<int> DropdVBloodPercentage;
        public static ConfigEntry<int> IncrementPercentageDropEveryTenLevelsVBlood;
        public static ConfigEntry<int> DropVBloodCurrenciesMin;
        public static ConfigEntry<int> DropVBloodCurrenciesMax;
        public static ConfigEntry<int> MaxCurrenciesPerDayPerPlayerVBlood;

        // PVP CONFIG DROP
        public static ConfigEntry<int> DropPvpPercentage;
        public static ConfigEntry<int> IncrementPercentageDropEveryTenLevelsPvp;
        public static ConfigEntry<int> DropPvpCurrenciesMin;
        public static ConfigEntry<int> DropPvpCurrenciesMax;
        public static ConfigEntry<int> MaxCurrenciesPerDayPerPlayerPvp;

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
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            GameData.OnInitialize += GameDataOnInitialize;
            GameData.OnDestroy += GameDataOnDestroy;

            if (VWorld.IsServer)
            {
                InitConfigServer();
                BloodyShop.serverInitMod(_harmony);
                //ServerEvents.OnGameDataInitialized += GameDataOnInitialize;
            }
            else
            {
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
            
            GameData.OnDestroy -= GameDataOnDestroy;
            GameData.OnInitialize -= GameDataOnInitialize;
            return true;
        }

        private static void GameDataOnInitialize(World world)
        {
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



        private void InitConfigServer()
        {
            ShopEnabled = Config.Bind("ConfigShop", "enabled", true, "Enable Shop");
            StoreName = Config.Bind("ConfigShop", "name", "Bloody Shop", "Store's name");
            AnnounceAddRemovePublic = Config.Bind("ConfigShop", "announceAddRemovePublic", true, "Public announcement when an item is added or removed from the store");
            AnnounceBuyPublic = Config.Bind("ConfigShop", "announceBuyPublic", true, "Public announcement when someone buys an item from the store");

            // DROP SYSTEM CONFIG
            DropEnabled = Config.Bind("DropSystem", "enabled", true, "Enable Drop System");

            // NPC DROP CONFIG
            DropNpcPercentage = Config.Bind("DropSystem", "minPercentageDropNpc", 5, "Percent chance that an NPC will drop the type of currency from the shop");
            IncrementPercentageDropEveryTenLevelsNpc = Config.Bind("DropSystem", "IncrementPercentageDropEveryTenLevelsNpc", 5, "Percentage increase for every rank of 10 levels of the NPC");
            DropdNpcCurrenciesMin = Config.Bind("DropSystem", "DropdNpcCurrenciesMin", 1, "Minimum currency an NPC can drop");
            DropNpcCurrenciesMax = Config.Bind("DropSystem", "DropNpcCurrenciesMax", 5, "Maximum currency an NPC can drop");
            MaxCurrenciesPerDayPerPlayerNpc = Config.Bind("DropSystem", "MaxCurrenciesPerDayPerPlayerNpc", 5, "Maximum number of currency that a user can get per day by NPC death");

            // VBLOOD DROP CONFIG
            DropdVBloodPercentage = Config.Bind("DropSystem", "minPercentageDropVBlood", 20, "Percent chance that an VBlood will drop the type of currency from the shop");
            IncrementPercentageDropEveryTenLevelsVBlood = Config.Bind("DropSystem", "IncrementPercentageDropEveryTenLevelsVBlood", 5, "Percentage increase for every rank of 10 levels of the VBlood");
            DropVBloodCurrenciesMin = Config.Bind("DropSystem", "DropVBloodCurrenciesMin", 10, "Minimum currency an VBlood can drop");
            DropVBloodCurrenciesMax = Config.Bind("DropSystem", "DropVBloodCurrenciesMax", 20, "Maximum currency an VBlood can drop");
            MaxCurrenciesPerDayPerPlayerVBlood = Config.Bind("DropSystem", "MaxCurrenciesPerDayPerPlayerVBlood", 20, "Maximum number of currency that a user can get per day by VBlood death");

            // PVP DROP CONFIG
            DropPvpPercentage = Config.Bind("DropSystem", "minPercentageDropPvp", 100, "Percent chance that victory in a PVP duel will drop the type of currency in the store");
            IncrementPercentageDropEveryTenLevelsPvp = Config.Bind("DropSystem", "IncrementPercentageDropEveryTenLevelsPvp", 5, "Percentage increase for every rank of 10 levels of the Player killed in pvp duel");
            DropPvpCurrenciesMin = Config.Bind("DropSystem", "DropPvpCurrenciesMin", 15, "Minimum currency can drop victory in PVP");
            DropPvpCurrenciesMax = Config.Bind("DropSystem", "DropPvpCurrenciesMax", 20, "Maximum currency can drop victory in PVP");
            MaxCurrenciesPerDayPerPlayerPvp = Config.Bind("DropSystem", "MaxCurrenciesPerDayPerPlayerPvp", 20, "Maximum number of currency that a user can get per day by victory in PVP");
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
