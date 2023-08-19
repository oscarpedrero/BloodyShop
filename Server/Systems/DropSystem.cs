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
using Bloodstone.API;

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
            var userModelKiller = GameData.Users.FromEntity(playerCharacterKiller.UserEntity);

            //Plugin.Logger.LogInfo($"PVE DROP");

            UnitLevel UnitDiedLevel = em.GetComponentData<UnitLevel>(died);


            var diedLevel = UnitDiedLevel.Level;

           // Plugin.Logger.LogInfo($"NPC Level {diedLevel}");

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
            var userModelKiller = GameData.Users.FromEntity(playerCharacterKiller.UserEntity);

            var currencies = ShareDB.getCurrencyList();
            var random = new Random();
            int indexCurrencies = random.Next(currencies.Count);

            var prefabCurrencyGUID = new PrefabGUID(currencies[indexCurrencies].guid);

            //Plugin.Logger.LogInfo($"PVP DROP");
            
            var playerCharacterDied = em.GetComponentData<PlayerCharacter>(died);
            var userModelDied = GameData.Users.FromEntity(playerCharacterKiller.UserEntity);
            var diedLevel = userModelDied.Character.Equipment.Level;

            //Plugin.Logger.LogInfo($"User Died Level {diedLevel}");

            var percentFinal = calculateDropPercentage((int) diedLevel, ConfigDB.DropPvpPercentage, ConfigDB.IncrementPercentageDropEveryTenLevelsPvp);
            if (probabilityOeneratingReward(percentFinal))
            {
                var totalCurrencies = rnd.Next(ConfigDB.DropPvpCurrenciesMin, ConfigDB.DropPvpCurrenciesMax);
                if (ConfigDB.searchUserCurrencyPerDay(userModelKiller.CharacterName, out UserCurrenciesPerDayModel userCurrenciesPerDay))
                {
                    var virtualAmount = userCurrenciesPerDay.AmountPvp + totalCurrencies;
                    if (virtualAmount <= ConfigDB.MaxCurrenciesPerDayPerPlayerPvp)
                    {
                        userCurrenciesPerDay.AmountPvp = virtualAmount;
                        userModelKiller.DropItemNearby(prefabCurrencyGUID, totalCurrencies);

                        ConfigDB.addUserCurrenciesPerDayToList(userCurrenciesPerDay);
                        SaveDataToFiles.saveUsersCurrenciesPerDay();
                        //Plugin.Logger.LogInfo($"Drop PVP {totalCurrencies} currencies");
                        return;
                    }
                    else if (userCurrenciesPerDay.AmountNpc < ConfigDB.MaxCurrenciesPerDayPerPlayerPvp)
                    {
                        totalCurrencies = ConfigDB.MaxCurrenciesPerDayPerPlayerPvp - userCurrenciesPerDay.AmountPvp;
                        userCurrenciesPerDay.AmountPvp += totalCurrencies;
                        userModelKiller.DropItemNearby(prefabCurrencyGUID, totalCurrencies);

                        ConfigDB.addUserCurrenciesPerDayToList(userCurrenciesPerDay);
                        SaveDataToFiles.saveUsersCurrenciesPerDay();
                        //Plugin.Logger.LogInfo($"Drop PVP {totalCurrencies} currencies");
                        return;
                    }
                }
            }


        }

        private static void rewardForNPC(UserModel userModelKiller, int diedLevel)
        {
            var currencies = ShareDB.getCurrencyList();
            var random = new Random();
            int indexCurrencies = random.Next(currencies.Count);

            var prefabCurrencyGUID = new PrefabGUID(currencies[indexCurrencies].guid);

            var percentFinal = calculateDropPercentage(diedLevel, ConfigDB.DropNpcPercentage, ConfigDB.IncrementPercentageDropEveryTenLevelsNpc);
            if (probabilityOeneratingReward(percentFinal))
            {
                var totalCurrencies = rnd.Next(ConfigDB.DropdNpcCurrenciesMin, ConfigDB.DropNpcCurrenciesMax);
                if (ConfigDB.searchUserCurrencyPerDay(userModelKiller.CharacterName,out UserCurrenciesPerDayModel userCurrenciesPerDay))
                {
                    var virtualAmount = userCurrenciesPerDay.AmountNpc + totalCurrencies;
                    if (virtualAmount <= ConfigDB.MaxCurrenciesPerDayPerPlayerNpc)
                    {
                        userCurrenciesPerDay.AmountNpc = virtualAmount;
                        userModelKiller.DropItemNearby(prefabCurrencyGUID, totalCurrencies);

                        ConfigDB.addUserCurrenciesPerDayToList(userCurrenciesPerDay);
                        SaveDataToFiles.saveUsersCurrenciesPerDay();
                        //Plugin.Logger.LogInfo($"Drop NPC {totalCurrencies} currencies");
                        return;
                    } else if (userCurrenciesPerDay.AmountNpc < ConfigDB.MaxCurrenciesPerDayPerPlayerNpc)
                    {
                        totalCurrencies = ConfigDB.MaxCurrenciesPerDayPerPlayerNpc - userCurrenciesPerDay.AmountNpc;
                        userCurrenciesPerDay.AmountNpc += totalCurrencies;
                        userModelKiller.DropItemNearby(prefabCurrencyGUID, totalCurrencies);

                        ConfigDB.addUserCurrenciesPerDayToList(userCurrenciesPerDay);
                        SaveDataToFiles.saveUsersCurrenciesPerDay();
                       // Plugin.Logger.LogInfo($"Drop NPC {totalCurrencies} currencies");
                        return;
                    }
                }
                    
                
            }
        }

        private static void rewardForVBlood(UserModel userModelKiller, int diedLevel)
        {
            var currencies = ShareDB.getCurrencyList();
            var random = new Random();
            int indexCurrencies = random.Next(currencies.Count);

            var prefabCurrencyGUID = new PrefabGUID(currencies[indexCurrencies].guid);

            var percentFinal = calculateDropPercentage(diedLevel, ConfigDB.DropdVBloodPercentage, ConfigDB.IncrementPercentageDropEveryTenLevelsVBlood);
            if (probabilityOeneratingReward(percentFinal))
            {
                var totalCurrencies = rnd.Next(ConfigDB.DropVBloodCurrenciesMin, ConfigDB.DropVBloodCurrenciesMax);
                if (ConfigDB.searchUserCurrencyPerDay(userModelKiller.CharacterName, out UserCurrenciesPerDayModel userCurrenciesPerDay))
                {
                    var virtualAmount = userCurrenciesPerDay.AmountVBlood + totalCurrencies;
                    if (virtualAmount <= ConfigDB.MaxCurrenciesPerDayPerPlayerVBlood)
                    {
                        userCurrenciesPerDay.AmountVBlood = virtualAmount;
                        userModelKiller.DropItemNearby(prefabCurrencyGUID, totalCurrencies);

                        ConfigDB.addUserCurrenciesPerDayToList(userCurrenciesPerDay);
                        SaveDataToFiles.saveUsersCurrenciesPerDay();
                        //Plugin.Logger.LogInfo($"Drop NPC {totalCurrencies} currencies");
                        return;
                    }
                    else if (userCurrenciesPerDay.AmountVBlood < ConfigDB.MaxCurrenciesPerDayPerPlayerVBlood)
                    {
                        totalCurrencies = ConfigDB.MaxCurrenciesPerDayPerPlayerVBlood - userCurrenciesPerDay.AmountVBlood;
                        userCurrenciesPerDay.AmountVBlood += totalCurrencies;
                        userModelKiller.DropItemNearby(prefabCurrencyGUID, totalCurrencies);

                        ConfigDB.addUserCurrenciesPerDayToList(userCurrenciesPerDay);
                        SaveDataToFiles.saveUsersCurrenciesPerDay();
                        //Plugin.Logger.LogInfo($"Drop NPC {totalCurrencies} currencies");
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
                //Plugin.Logger.LogInfo($"Drop {number} <= {percentage}");
                return true;
            }

            return false;
        }
    }
}
