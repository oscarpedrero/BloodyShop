using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using VRising.GameData;
using HarmonyLib;
using Unity.Entities;
using UnityEngine;
using Wetstone.API;
using System;

namespace BloodyShop
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("xyz.molenzwiebel.wetstone")]
    [BepInDependency("gg.deca.VampireCommandFramework", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BasePlugin, IRunOnInitialized
    {

        internal static Plugin Instance;
        internal static string Name = PluginInfo.PLUGIN_NAME;
        internal static string Guid = PluginInfo.PLUGIN_GUID;
        internal static string Version = PluginInfo.PLUGIN_VERSION;

        public static ManualLogSource Logger;
        private Harmony _harmony;

        public static ConfigEntry<bool> ShopEnabled;
        public static ConfigEntry<bool> AnnounceAddRemovePublic;
        public static ConfigEntry<bool> AnnounceBuyPublic;
        public static ConfigEntry<int> CoinGUID;
        public static ConfigEntry<string> StoreName;


        /// 
        /// DROP SYSTEM
        /// 

        public static ConfigEntry<bool> DropEnabled;

        // NPC CONFIG DROP
        public static ConfigEntry<int> DropNpcPercentage;
        public static ConfigEntry<int> IncrementPercentageDropEveryTenLevelsNpc;
        public static ConfigEntry<int> DropdNpcCoinsMin;
        public static ConfigEntry<int> DropNpcCoinsMax;
        public static ConfigEntry<int> MaxCoinsPerDayPerPlayerNpc;

        // VBLOOD CONFIG DROP
        public static ConfigEntry<int> DropdVBloodPercentage;
        public static ConfigEntry<int> IncrementPercentageDropEveryTenLevelsVBlood;
        public static ConfigEntry<int> DropVBloodCoinsMin;
        public static ConfigEntry<int> DropVBloodCoinsMax;
        public static ConfigEntry<int> MaxCoinsPerDayPerPlayerVBlood;

        // PVP CONFIG DROP
        public static ConfigEntry<int> DropPvpPercentage;
        public static ConfigEntry<int> IncrementPercentageDropEveryTenLevelsPvp;
        public static ConfigEntry<int> DropPvpCoinsMin;
        public static ConfigEntry<int> DropPvpCoinsMax;
        public static ConfigEntry<int> MaxCoinsPerDayPerPlayerPvp;

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
                var vcfFound = IL2CPPChainloader.Instance.Plugins.TryGetValue("gg.deca.VampireCommandFramework", out var info);
                if (!vcfFound)
                {
                    Logger.LogError("VampireCommandFramework not found! This is required for the server.");
                    return;
                }
                BloodyShop.serverInitMod(_harmony);
            }
            else
            {
                BloodyShop.clientInitMod(_harmony);
            }

            // Plugin startup logic
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            GameData.OnInitialize += GameDataOnInitialize;
            GameData.OnDestroy += GameDataOnDestroy;

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
                    Logger.LogInfo($"Error GameDataOnInitialize {error.Message}");
                }
            }
        }

        private static void GameDataOnDestroy()
        {
            Logger.LogInfo("GameDataOnDestroy");
        }


        private void InitConfigServer()
        {
            ShopEnabled = Config.Bind("ConfigShop", "enabled", true, "Enable Shop");
            StoreName = Config.Bind("ConfigShop", "name", "Bloody Shop", "Store's name");
            CoinGUID = Config.Bind("ConfigShop", "coinGUID", -949672483, "Item that will be used as currency within the service, by default they are silver coins, if you want to change the item you must include the GUID of said object that you can get from https://gaming.tools/v-rising/items");
            AnnounceAddRemovePublic = Config.Bind("ConfigShop", "announceAddRemovePublic", true, "Public announcement when an item is added or removed from the store");
            AnnounceBuyPublic = Config.Bind("ConfigShop", "announceBuyPublic", true, "Public announcement when someone buys an item from the store");

            // DROP SYSTEM CONFIG
            DropEnabled = Config.Bind("DropSystem", "enabled", true, "Enable Drop System");

            // NPC DROP CONFIG
            DropNpcPercentage = Config.Bind("DropSystem", "minPercentageDropNpc", 5, "Percent chance that an NPC will drop the type of currency from the shop");
            IncrementPercentageDropEveryTenLevelsNpc = Config.Bind("DropSystem", "IncrementPercentageDropEveryTenLevelsNpc", 5, "Percentage increase for every rank of 10 levels of the NPC");
            DropdNpcCoinsMin = Config.Bind("DropSystem", "DropdNpcCoinsMin", 1, "Minimum currency an NPC can drop");
            DropNpcCoinsMax = Config.Bind("DropSystem", "DropNpcCoinsMax", 5, "Maximum currency an NPC can drop");
            MaxCoinsPerDayPerPlayerNpc = Config.Bind("DropSystem", "MaxCoinsPerDayPerPlayerNpc", 5, "Maximum number of currency that a user can get per day by NPC death");

            // VBLOOD DROP CONFIG
            DropdVBloodPercentage = Config.Bind("DropSystem", "minPercentageDropVBlood", 20, "Percent chance that an VBlood will drop the type of currency from the shop");
            IncrementPercentageDropEveryTenLevelsVBlood = Config.Bind("DropSystem", "IncrementPercentageDropEveryTenLevelsVBlood", 5, "Percentage increase for every rank of 10 levels of the VBlood");
            DropVBloodCoinsMin = Config.Bind("DropSystem", "DropVBloodCoinsMin", 10, "Minimum currency an VBlood can drop");
            DropVBloodCoinsMax = Config.Bind("DropSystem", "DropVBloodCoinsMax", 20, "Maximum currency an VBlood can drop");
            MaxCoinsPerDayPerPlayerVBlood = Config.Bind("DropSystem", "MaxCoinsPerDayPerPlayerVBlood", 20, "Maximum number of currency that a user can get per day by VBlood death");

            // PVP DROP CONFIG
            DropPvpPercentage = Config.Bind("DropSystem", "minPercentageDropPvp", 100, "Percent chance that victory in a PVP duel will drop the type of currency in the store");
            IncrementPercentageDropEveryTenLevelsPvp = Config.Bind("DropSystem", "IncrementPercentageDropEveryTenLevelsPvp", 5, "Percentage increase for every rank of 10 levels of the Player killed in pvp duel");
            DropPvpCoinsMin = Config.Bind("DropSystem", "DropPvpCoinsMin", 15, "Minimum currency can drop victory in PVP");
            DropPvpCoinsMax = Config.Bind("DropSystem", "DropPvpCoinsMax", 20, "Maximum currency can drop victory in PVP");
            MaxCoinsPerDayPerPlayerPvp = Config.Bind("DropSystem", "MaxCoinsPerDayPerPlayerPvp", 20, "Maximum number of currency that a user can get per day by victory in PVP");
        }

        public void OnGameInitialized()
        {

            Logger.LogInfo("OnGameInitialized");
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
