using BloodyShop.DB;
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
    public class RewardSystem
    {
        private static EntityManager em = VWorld.Server.EntityManager;

        private static Random rnd = new Random();

        private static PrefabGUID vBloodType = new PrefabGUID(1557174542);

        public static void ServerEvents_OnDeath(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
        {
            foreach (var deathEvent in deathEvents)
            {
                if (em.HasComponent<PlayerCharacter>(deathEvent.Killer) && em.HasComponent<Movement>(deathEvent.Died))
                {
                    bool isNPC = em.HasComponent<UnitLevel>(deathEvent.Died);
                    if (isNPC)
                    {
                        pveReward(deathEvent.Killer, deathEvent.Died);
                    } else
                    {
                        pvpReward(deathEvent.Killer, deathEvent.Died);
                    }
                }
            }
        }

        private static void pveReward(Entity killer, Entity died)
        {
            if (em.HasComponent<Minion>(died)) return;

            Plugin.Logger.LogInfo($"PVE REWARD");

            var playerCharacterKiller = em.GetComponentData<PlayerCharacter>(killer);
            var userModelKiller = GameData.Users.FromEntity(playerCharacterKiller.UserEntity._Entity);

            UnitLevel UnitDiedLevel = em.GetComponentData<UnitLevel>(died);
            var diedLevel = UnitDiedLevel.Level;

            Plugin.Logger.LogInfo($"NPC Level {diedLevel}");

            var killerLevel = userModelKiller.Character.Equipment.Level;

            Plugin.Logger.LogInfo($"User Level {killerLevel}");

            var difference = killerLevel - diedLevel;

            Plugin.Logger.LogInfo($"Difference {difference}");

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
                rewardForVBlood(userModelKiller, difference);
            } else
            {
                rewardForNPC(userModelKiller, difference);
            }

        }

        private static void pvpReward(Entity killer, Entity died)
        {

            if (em.HasComponent<Minion>(died)) return;

            Plugin.Logger.LogInfo($"PVP REWARD");

            var playerCharacterKiller = em.GetComponentData<PlayerCharacter>(killer);
            var userModelKiller = GameData.Users.FromEntity(playerCharacterKiller.UserEntity._Entity);
            var killerLevel = userModelKiller.Character.Equipment.Level;

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
            }
        }

        private static void rewardForNPC(UserModel userModelKiller, float difference)
        {
            if (difference >= -14 && difference <= 25)
            {

                Plugin.Logger.LogInfo($"NPC Reward 25% 1 to 5 Coins");

                var prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());
                if (probabilityOeneratingReward(25))
                {
                    var totalCoins = rnd.Next(1, 5);
                    userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);
                }

            }
            else if (difference <= -15)
            {
                Plugin.Logger.LogInfo($"NPC Reward 5 Coins");

                var prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());
                userModelKiller.DropItemNearby(prefabCoinGUID, 5);
            }
        }

        private static void rewardForVBlood(UserModel userModelKiller, float difference)
        {
            if (difference >= -9 && difference <= 5)
            {

                Plugin.Logger.LogInfo($"VBlood Reward 50% 1 to 5 Coins");

                var prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());
                if (probabilityOeneratingReward(50))
                {
                    var totalCoins = 10;
                    userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);
                }

            }
            else if (difference <= -10)
            {
                Plugin.Logger.LogInfo($"VBlood Reward 20 Coins");
                var prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());
                userModelKiller.DropItemNearby(prefabCoinGUID, 20);
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
