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
}