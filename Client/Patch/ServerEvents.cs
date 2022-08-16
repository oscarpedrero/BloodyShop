using HarmonyLib;
using ProjectM;
using System;
using Unity.Collections;
using Unity.Entities;

namespace BloodyShop.Client.Patch;

public delegate void OnServerGameDataInitializedEventHandler(World world);
public delegate void OnUpdateEventHandler(World world);
public delegate void OnUnitSpawnedEventHandler(World world, Entity entity);
public delegate void DeathEventHandler(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents);

public static class ServerEvents
{
    public static event OnUpdateEventHandler OnUpdate;
    public static event DeathEventHandler OnDeath;
    public static event OnUnitSpawnedEventHandler OnUnitSpawned;
    public static event OnServerGameDataInitializedEventHandler OnGameDataInitialized;

    [HarmonyPatch(typeof(LoadPersistenceSystemV2), nameof(LoadPersistenceSystemV2.SetLoadState))]
    [HarmonyPrefix]
    private static void ServerStartupStateChange_Prefix(ServerStartupState.State loadState, LoadPersistenceSystemV2 __instance)
    {
        try
        {
            if (loadState == ServerStartupState.State.SuccessfulStartup)
            {
                OnGameDataInitialized?.Invoke(__instance.World);
            }
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
        }
    }

    [HarmonyPatch(typeof(ServerTimeSystem_Server), nameof(ServerTimeSystem_Server.OnUpdate))]
    [HarmonyPostfix]
    private static void ServerTimeSystemOnUpdate_Postfix(ServerTimeSystem_Server __instance)
    {
        try
        {
            OnUpdate?.Invoke(__instance.World);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
        }
    }


    [HarmonyPatch(typeof(DeathEventListenerSystem), nameof(DeathEventListenerSystem.OnUpdate))]
    [HarmonyPostfix]
    private static void DeathEventListenerSystemPatch_Postfix(DeathEventListenerSystem __instance)
    {
        try
        {
            var deathEvents =
                __instance._DeathEventQuery.ToComponentDataArray<DeathEvent>(Allocator.Temp);
            if (deathEvents.Length > 0)
            {
                OnDeath?.Invoke(__instance, deathEvents);
            }
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
        }
    }

    [HarmonyPatch(typeof(UnitSpawnerReactSystem), nameof(UnitSpawnerReactSystem.OnUpdate))]
    [HarmonyPostfix]
    public static void Prefix(UnitSpawnerReactSystem __instance)
    {
        var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
        foreach (var entity in entities)
        {
            try
            {
                OnUnitSpawned?.Invoke(__instance.World, entity);
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }
    }
}