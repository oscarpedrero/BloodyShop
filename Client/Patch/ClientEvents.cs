using BloodyShop.Client.UI;
using HarmonyLib;
using ProjectM;
using System;
using Unity.Entities;

namespace BloodyShop.Client.Patch;

public delegate void OnClientGameDataInitializedEventHandler(World world);

public static class ClientEvents
{
    public static event OnClientGameDataInitializedEventHandler OnGameDataInitialized;

    private static bool _onGameDataInitializedTriggered;
    [HarmonyPatch(typeof(GameDataManager), "OnUpdate")]
    [HarmonyPostfix]
    private static void GameDataManagerOnUpdatePostfix(GameDataManager __instance)
    {
        if (_onGameDataInitializedTriggered)
        {
            return;
        }
        try
        {
            if (!__instance.GameDataInitialized) return;
            _onGameDataInitializedTriggered= true;
            OnGameDataInitialized?.Invoke(__instance.World);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
        }
    }

    [HarmonyPatch(typeof(ClientBootstrapSystem), nameof(ClientBootstrapSystem.OnClientDisconnected))]
    [HarmonyPostfix]
    private static void ClientBootstrapSystemDisconnectPostfix(ClientBootstrapSystem __instance)
    {
        Plugin.Logger.LogInfo("Disconnected");
        ClientMod.UIInit = false;
        UIManager.AdminPanel?.Destroy();
        UIManager.MenuPanel?.Destroy();
        UIManager.ShopPanel?.Destroy();
    }
}