using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using Wetstone.API;

namespace BloodyShop.Server.Patch
{

    public delegate void DeathEventHandler(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents);
    [HarmonyPatch]
    public class ServerEvents
    {

        public static event DeathEventHandler OnDeath;

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
    }
}
