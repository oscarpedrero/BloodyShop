using BloodyShop.DB;
using BloodyShop.Server.DB;
using BloodyShop.Server.DB.Model;
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
                }
            }
        }

        public static void ServerEvents_OnVampireDowned(VampireDownedServerEventSystem sender, NativeArray<Entity> vampireDownedEntitys)
        {
            if (!ConfigDB.DropEnabled) return;

            foreach (var entity in vampireDownedEntitys)
            {
                VampireDownedServerEventSystem.TryFindRootOwner(entity, 1, em, out var Died);
                Entity Source = em.GetComponentData<VampireDownedBuff>(entity).Source;
                VampireDownedServerEventSystem.TryFindRootOwner(Source, 1, em, out var Killer);

                if (em.HasComponent<PlayerCharacter>(Killer) && em.HasComponent<PlayerCharacter>(Died) && !Killer.Equals(Died))
                {
                    pvpReward(Killer, Died);
                }
            }
        }

        private static void pveReward(Entity killer, Entity died)
        {
            if (em.HasComponent<Minion>(died)) return;

            var playerCharacterKiller = em.GetComponentData<PlayerCharacter>(killer);
            var userModelKiller = GameData.Users.FromEntity(playerCharacterKiller.UserEntity._Entity);

            Plugin.Logger.LogInfo($"PVE DROP");

            UnitLevel UnitDiedLevel = em.GetComponentData<UnitLevel>(died);


            var diedLevel = UnitDiedLevel.Level;

            Plugin.Logger.LogInfo($"NPC Level {diedLevel}");

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
                rewardForVBlood(userModelKiller, diedLevel);
            } else
            {
                rewardForNPC(userModelKiller, diedLevel);
            }

        }

        private static void pvpReward(Entity killer, Entity died)
        {

            if (em.HasComponent<Minion>(died)) return;

            var playerCharacterKiller = em.GetComponentData<PlayerCharacter>(killer);
            var userModelKiller = GameData.Users.FromEntity(playerCharacterKiller.UserEntity._Entity);

            var prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());

            Plugin.Logger.LogInfo($"PVP DROP");
            
            var playerCharacterDied = em.GetComponentData<PlayerCharacter>(died);
            var userModelDied = GameData.Users.FromEntity(playerCharacterKiller.UserEntity._Entity);
            var diedLevel = userModelDied.Character.Equipment.Level;

            Plugin.Logger.LogInfo($"User Died Level {diedLevel}");

            var percentFinal = calculateDropPercentage((int) diedLevel, ConfigDB.DropPvpPercentage, ConfigDB.IncrementPercentageDropEveryTenLevelsPvp);
            if (probabilityOeneratingReward(percentFinal))
            {
                var totalCoins = rnd.Next(ConfigDB.DropPvpCoinsMin, ConfigDB.DropPvpCoinsMax);
                if (ConfigDB.searchUserCoinPerDay(userModelKiller.CharacterName, out UserCoinsPerDayModel userCoinsPerDay))
                {
                    var virtualAmount = userCoinsPerDay.AmountPvp + totalCoins;
                    if (virtualAmount <= ConfigDB.MaxCoinsPerDayPerPlayerPvp)
                    {
                        userCoinsPerDay.AmountPvp = virtualAmount;
                        userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);

                        ConfigDB.addUserCoinsPerDayToList(userCoinsPerDay);
                        SaveDataToFiles.saveUsersCoinsPerDay();
                        Plugin.Logger.LogInfo($"Drop PVP {totalCoins} coins");
                        return;
                    }
                    else if (userCoinsPerDay.AmountNpc < ConfigDB.MaxCoinsPerDayPerPlayerPvp)
                    {
                        totalCoins = ConfigDB.MaxCoinsPerDayPerPlayerPvp - userCoinsPerDay.AmountPvp;
                        userCoinsPerDay.AmountPvp += totalCoins;
                        userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);

                        ConfigDB.addUserCoinsPerDayToList(userCoinsPerDay);
                        SaveDataToFiles.saveUsersCoinsPerDay();
                        Plugin.Logger.LogInfo($"Drop PVP {totalCoins} coins");
                        return;
                    }
                }
            }


        }

        private static void rewardForNPC(UserModel userModelKiller, int diedLevel)
        {
            var prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());
            var percentFinal = calculateDropPercentage(diedLevel, ConfigDB.DropNpcPercentage, ConfigDB.IncrementPercentageDropEveryTenLevelsNpc);
            if (probabilityOeneratingReward(percentFinal))
            {
                var totalCoins = rnd.Next(ConfigDB.DropdNpcCoinsMin, ConfigDB.DropNpcCoinsMax);
                if (ConfigDB.searchUserCoinPerDay(userModelKiller.CharacterName,out UserCoinsPerDayModel userCoinsPerDay))
                {
                    var virtualAmount = userCoinsPerDay.AmountNpc + totalCoins;
                    if (virtualAmount <= ConfigDB.MaxCoinsPerDayPerPlayerNpc)
                    {
                        userCoinsPerDay.AmountNpc = virtualAmount;
                        userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);

                        ConfigDB.addUserCoinsPerDayToList(userCoinsPerDay);
                        SaveDataToFiles.saveUsersCoinsPerDay();
                        Plugin.Logger.LogInfo($"Drop NPC {totalCoins} coins");
                        return;
                    } else if (userCoinsPerDay.AmountNpc < ConfigDB.MaxCoinsPerDayPerPlayerNpc)
                    {
                        totalCoins = ConfigDB.MaxCoinsPerDayPerPlayerNpc - userCoinsPerDay.AmountNpc;
                        userCoinsPerDay.AmountNpc += totalCoins;
                        userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);

                        ConfigDB.addUserCoinsPerDayToList(userCoinsPerDay);
                        SaveDataToFiles.saveUsersCoinsPerDay();
                        Plugin.Logger.LogInfo($"Drop NPC {totalCoins} coins");
                        return;
                    }
                }
                    
                
            }
        }

        private static void rewardForVBlood(UserModel userModelKiller, int diedLevel)
        {
            var prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());
            var percentFinal = calculateDropPercentage(diedLevel, ConfigDB.DropdVBloodPercentage, ConfigDB.IncrementPercentageDropEveryTenLevelsVBlood);
            if (probabilityOeneratingReward(percentFinal))
            {
                var totalCoins = rnd.Next(ConfigDB.DropVBloodCoinsMin, ConfigDB.DropVBloodCoinsMax);
                if (ConfigDB.searchUserCoinPerDay(userModelKiller.CharacterName, out UserCoinsPerDayModel userCoinsPerDay))
                {
                    var virtualAmount = userCoinsPerDay.AmountVBlood + totalCoins;
                    if (virtualAmount <= ConfigDB.MaxCoinsPerDayPerPlayerVBlood)
                    {
                        userCoinsPerDay.AmountVBlood = virtualAmount;
                        userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);

                        ConfigDB.addUserCoinsPerDayToList(userCoinsPerDay);
                        SaveDataToFiles.saveUsersCoinsPerDay();
                        Plugin.Logger.LogInfo($"Drop NPC {totalCoins} coins");
                        return;
                    }
                    else if (userCoinsPerDay.AmountVBlood < ConfigDB.MaxCoinsPerDayPerPlayerVBlood)
                    {
                        totalCoins = ConfigDB.MaxCoinsPerDayPerPlayerVBlood - userCoinsPerDay.AmountVBlood;
                        userCoinsPerDay.AmountVBlood += totalCoins;
                        userModelKiller.DropItemNearby(prefabCoinGUID, totalCoins);

                        ConfigDB.addUserCoinsPerDayToList(userCoinsPerDay);
                        SaveDataToFiles.saveUsersCoinsPerDay();
                        Plugin.Logger.LogInfo($"Drop NPC {totalCoins} coins");
                        return;
                    }
                }
            }
        }

        private static int calculateDropPercentage(int level, int initialPercent, int incremental)
        {
            decimal tensDecimal = level / 10;
            decimal tens = Math.Ceiling(tensDecimal);

            return Decimal.ToInt32(tens * incremental) + initialPercent;
        }

        private static bool probabilityOeneratingReward(int percentage)
        {
            var number = rnd.Next(1, 100);

            if(number <= percentage)
            {
                Plugin.Logger.LogInfo($"Drop {number} <= {percentage}");
                return true;
            }

            return false;
        }
    }
}
