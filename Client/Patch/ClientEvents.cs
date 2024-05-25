/*using BloodyShop.Client.UI;
using HarmonyLib;
using ProjectM;
using ProjectM.UI;
using Stunlock.Network;
using System;
using Unity.Entities;

namespace BloodyShop.Client.Patch;

public delegate void OnClientConnectedEventHandler();
public delegate void OnClientDisconectedEventHandler();


public static class ClientEvents
{
    public static event OnClientConnectedEventHandler OnClientConnected;
    public static event OnClientDisconectedEventHandler OnClientDisconected;

    private static bool _onUserDisconectTriggered;
    private static bool _onUserConnectedtTriggered;


    [HarmonyPatch(typeof(ClientBootstrapSystem), nameof(ClientBootstrapSystem.OnUpdate))]
    [HarmonyPrefix]
    public static void OnClientConnectedPostfix(ClientBootstrapSystem __instance)
    {

        if (_onUserConnectedtTriggered)
        {
            return;
        }
        try
        {
            if (__instance.ConnectionStatus.ToString() != "Connected") return;
            _onUserConnectedtTriggered = true;
            _onUserDisconectTriggered = false;
            OnClientConnected?.Invoke();
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
        }
    
    }

    [HarmonyPatch(typeof(ClientBootstrapSystem), nameof(ClientBootstrapSystem.OnDestroy))]
    [HarmonyPrefix]
    public static void ClientBootstrapSystemOnDestroyPostfix(ClientBootstrapSystem __instance)
    {
        if (_onUserDisconectTriggered)
        {
            return;
        }
        try
        {
            if (__instance.ConnectionStatus.ToString() != "Connected") return;
            _onUserDisconectTriggered = true;
            _onUserConnectedtTriggered = false;
            OnClientDisconected?.Invoke();
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
        }
        

    }

}
*/