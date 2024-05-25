/*using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using Bloodstone.API;

namespace BloodyShop.Server.Patch
{

    public delegate void DeathEventHandler(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents);
    public delegate void VampireDownedHandler(VampireDownedServerEventSystem sender, NativeArray<Entity> deathEvents);
    [HarmonyPatch]
    public class ServerEvents
    {

        public static event DeathEventHandler OnDeath;
        public static event VampireDownedHandler OnVampireDowned;

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

        [HarmonyPatch(typeof(VampireDownedServerEventSystem), nameof(VampireDownedServerEventSystem.OnUpdate))]
        [HarmonyPostfix]
        public static void VampireDownedServerEventSystem_Postfix(VampireDownedServerEventSystem __instance)
        {

            if (__instance.__OnUpdate_LambdaJob0_entityQuery == null) return;

            EntityManager em = __instance.EntityManager;
            var vampireDownedEntitys = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
            if (vampireDownedEntitys.Length > 0)
            {
                OnVampireDowned?.Invoke(__instance, vampireDownedEntitys);
            }

        }
    }
}
*/
