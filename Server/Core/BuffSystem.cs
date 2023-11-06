using Bloodstone.API;
using ProjectM.Network;
using ProjectM.Shared;
using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace BloodyShop.Server.Core
{
    internal class BuffSystem
    {
        public const int NO_DURATION = 0;
        public const int DEFAULT_DURATION = -1;
        public const int RANDOM_POWER = -1;

        public static bool BuffPlayer(Entity character, Entity user, PrefabGUID buff, int duration = DEFAULT_DURATION, bool persistsThroughDeath = false)
        {

            ClearExtraBuffs(character);
            var des = VWorld.Server.GetExistingSystem<DebugEventsSystem>();
            var buffEvent = new ApplyBuffDebugEvent()
            {
                BuffPrefabGUID = buff
            };
            var fromCharacter = new FromCharacter()
            {
                User = user,
                Character = character
            };
            if (!BuffUtility.TryGetBuff(VWorld.Server.EntityManager, character, buff, out Entity buffEntity))
            {
                des.ApplyBuff(fromCharacter, buffEvent);
                if (BuffUtility.TryGetBuff(VWorld.Server.EntityManager, character, buff, out buffEntity))
                {
                    if (buffEntity.Has<CreateGameplayEventsOnSpawn>())
                    {
                        buffEntity.Remove<CreateGameplayEventsOnSpawn>();
                    }
                    if (buffEntity.Has<GameplayEventListeners>())
                    {
                        buffEntity.Remove<GameplayEventListeners>();
                    }

                    if (persistsThroughDeath)
                    {
                        buffEntity.Add<Buff_Persists_Through_Death>();
                        if (buffEntity.Has<RemoveBuffOnGameplayEvent>())
                        {
                            buffEntity.Remove<RemoveBuffOnGameplayEvent>();
                        }

                        if (buffEntity.Has<RemoveBuffOnGameplayEventEntry>())
                        {
                            buffEntity.Remove<RemoveBuffOnGameplayEventEntry>();
                        }
                    }
                    if (duration > 0 && duration != DEFAULT_DURATION)
                    {
                        if (buffEntity.Has<LifeTime>())
                        {
                            var lifetime = buffEntity.Read<LifeTime>();
                            lifetime.Duration = duration;
                            buffEntity.Write(lifetime);
                        }
                    }
                    else if (duration == NO_DURATION)
                    {
                        if (buffEntity.Has<LifeTime>())
                        {
                            var lifetime = buffEntity.Read<LifeTime>();
                            lifetime.Duration = -1;
                            lifetime.EndAction = LifeTimeEndAction.None;
                            buffEntity.Write(lifetime);
                            //buffEntity.Remove<LifeTime>();
                        }
                        if (buffEntity.Has<RemoveBuffOnGameplayEvent>())
                        {
                            buffEntity.Remove<RemoveBuffOnGameplayEvent>();
                        }
                        if (buffEntity.Has<RemoveBuffOnGameplayEventEntry>())
                        {
                            buffEntity.Remove<RemoveBuffOnGameplayEventEntry>();
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public static void ClearExtraBuffs(Entity player)
        {
            var buffs = VWorld.Server.EntityManager.GetBuffer<BuffBuffer>(player);
            var stringsToIgnore = new List<string>
        {
            "BloodBuff",
            "SetBonus",
            "EquipBuff",
            "Combat",
            "VBlood_Ability_Replace",
            "Shapeshift",
            "Interact",
            "AB_Consumable",
        };

            foreach (var buff in buffs)
            {
                bool shouldRemove = true;
                foreach (string word in stringsToIgnore)
                {
                    if (buff.PrefabGuid.LookupName().Contains(word))
                    {
                        shouldRemove = false;
                        break;
                    }
                }
                if (shouldRemove)
                {
                    DestroyUtility.Destroy(VWorld.Server.EntityManager, buff.Entity, DestroyDebugReason.TryRemoveBuff);
                }
            }
            var equipment = player.Read<Equipment>();
            if (!equipment.IsEquipped(new PrefabGUID(1063517722), out var equipmentType) && BuffUtility.HasBuff(VWorld.Server.EntityManager, player, new PrefabGUID(476186894)))
            {
                UnbuffCharacter(player, new PrefabGUID(476186894));
            }
        }

        public static void UnbuffCharacter(Entity Character, PrefabGUID buffGUID)
        {
            if (BuffUtility.TryGetBuff(VWorld.Server.EntityManager, Character, buffGUID, out var buffEntity))
            {
                DestroyUtility.Destroy(VWorld.Server.EntityManager, buffEntity, DestroyDebugReason.TryRemoveBuff);
            }
        }
    }
}
