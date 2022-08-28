using BloodyShop.DB;
using BloodyShop.Server.DB;
using ProjectM;
using System;
using Unity.Collections;
using Unity.Entities;
using VRising.GameData;
using VRising.GameData.Methods;
using VRising.GameData.Models;
using Wetstone.API;

namespace BloodyShop.Server.Systems
{
    public class DropSystem
    {
        private static EntityManager em = VWorld.Server.EntityManager;

        private static Random rnd = new Random();

        private static PrefabGUID vBloodType = new PrefabGUID(1557174542);

        public static void ServerEvents_OnDeath(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
        {
            if (!ConfigDB.DropEnabled) return;

            foreach (var deathEvent in deathEvents)
            {
                if (em.HasComponent<PlayerCharacter>(deathEvent.Killer) && em.HasComponent<Movement>(deathEvent.Died))
                {
                    bool isNPC = em.HasComponent<UnitLevel>(deathEvent.Died);
                    if (isNPC)
                    {
                        pveReward(deathEvent.Killer, deathEvent.Died);
                    }
                } else if (em.HasComponent<PlayerCharacter>(deathEvent.Killer) && em.HasComponent<PlayerCharacter>(deathEvent.Died))
                {
                    pvpReward(deathEvent.Killer, deathEvent.Died);
                }
            }
        }

        private static void pveReward(Entity killer, Entity died)
        {
            if (em.HasComponent<Minion>(died)) return;

            Plugin.Logger.LogInfo($"PVE REWARD");

            var playerCharacterKiller = em.GetComponentData<PlayerCharacter>(killer);
            var userModelKiller = GameData.Users.FromEntity(playerCharacterKiller.UserEntity._Entity);

            /*UnitLevel UnitDiedLevel = em.GetComponentData<UnitLevel>(died);
            var diedLevel = UnitDiedLevel.Level;

            Plugin.Logger.LogInfo($"NPC Level {diedLevel}");

            var killerLevel = userModelKiller.Character.Equipment.Level;

            Plugin.Logger.LogInfo($"User Level {killerLevel}");

            var difference = killerLevel - diedLevel;

            Plugin.Logger.LogInfo($"Difference {difference}");*/

            bool isVBlood;
            if (em.HasComponent<BloodConsumeSource>(died))
            {
                BloodConsumeSource BloodSource = em.GetComponentData<BloodConsumeSource>(died);
                isVBlood = BloodSource.UnitBloodType.Equals(vBloodType);
            }
            else
            {
                isVBlood = false;
            }

            if (isVBlood)
            {
                rewardForVBlood(userModelKiller);
            } else
            {
                rewardForNPC(userModelKiller);
            }

        }

        private static void pvpReward(Entity killer, Entity died)
        {

            if (em.HasComponent<Minion>(died)) return;

            Plugin.Logger.LogInfo($"PVP REWARD");

            var playerCharacterKiller = em.GetComponentData<PlayerCharacter>(killer);
            var userModelKiller = GameData.Users.FromEntity(playerCharacterKiller.UserEntity._Entity);

            var prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());
            if (probabilityOeneratingReward(ConfigDB.DropPvpPercentage))
            {
                var totalCoins = rnd.Next(ConfigDB.DropPvpCoinsMin, ConfigDB.DropPvpCoinsMax);
                userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);
            }

            /*var killerLevel = userModelKiller.Character.Equipment.Level;

            Plugin.Logger.LogInfo($"User Killer Level {killerLevel}");

            var playerCharacterDied = em.GetComponentData<PlayerCharacter>(died);
            var userModelDied = GameData.Users.FromEntity(playerCharacterKiller.UserEntity._Entity);
            var diedLevel = userModelDied.Character.Equipment.Level;

            Plugin.Logger.LogInfo($"User Died Level {diedLevel}");

            var difference = killerLevel - diedLevel;

            Plugin.Logger.LogInfo($"Difference {difference}");

            if (difference <= -15)
            {
                var prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());
                if (probabilityOeneratingReward(25))
                {
                    var totalCoins = rnd.Next(1, 5);
                    userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);
                }
            }*/


        }

        private static void rewardForNPC(UserModel userModelKiller)
        {
            var prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());
            if (probabilityOeneratingReward(ConfigDB.DropNpcPercentage))
            {
                var totalCoins = rnd.Next(ConfigDB.DropdNpcCoinsMin, ConfigDB.DropNpcCoinsMax);
                userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);
            }
        }

        private static void rewardForVBlood(UserModel userModelKiller)
        {
            var prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());
            if (probabilityOeneratingReward(ConfigDB.DropdVBloodPercentage))
            {
                var totalCoins = rnd.Next(ConfigDB.DropVBloodCoinsMin, ConfigDB.DropVBloodCoinsMax);
                userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);
            }
        }

        private static bool probabilityOeneratingReward(int percentage)
        {
            var number = rnd.Next(1, 100);

            if(number <= percentage)
            {
                return true;
            }

            return false;
        }
    }
}
