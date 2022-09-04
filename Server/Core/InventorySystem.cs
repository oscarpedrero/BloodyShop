using System;
using System.Collections.Generic;
using System.Text;
using BloodyShop.Server.Systems;
using Unity.Entities;
using Unity.Collections;
using ProjectM;
using VRising.GameData;
using VRising.GameData.Methods;
using BloodyShop.DB;
using BloodyShop.DB.Models;
using BloodyShop.Utils;
using VRising.GameData.Models;
using BloodyShop.Server.DB;
using BloodyShop.Server.Network;
using VampireCommandFramework;
using Wetstone.API;

namespace BloodyShop.Server.Core
{
    public  class InventorySystem
    {
        public static bool verifyHaveSuficientCoins(Entity playerCharacter, PrefabGUID prefabCoinGUID, int coins)
        {
            try
            {
                NativeArray<InventoryBuffer> inventory = new NativeArray<InventoryBuffer>();
                InventoryUtilities.TryGetInventory(Plugin.Server.EntityManager, playerCharacter, out inventory);
                var totalCoins = InventoryUtilities.ItemCount(ref inventory, prefabCoinGUID);
                if (totalCoins >= coins)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception error)
            {
                Plugin.Logger.LogInfo($"Error: {error.Message}");
                return false;
            }

        }

        public static bool getCoinsFromInventory(Entity playerCharacter, string characterName, string prefabCoinName, PrefabGUID prefabCoinGUID, int price)
        {

            try
            {
                var userData = GameData.Users.GetUserByCharacterName(characterName);

                var coins = price;

                int totalSlots = InventoryUtilities.GetItemSlots(Plugin.Server.EntityManager, playerCharacter);

                var gameData = Plugin.Server.GetExistingSystem<GameDataSystem>();

                for (int i = 0; i < totalSlots; i++)
                {
                    if (InventoryUtilities.TryGetItemAtSlot(Plugin.Server.EntityManager, playerCharacter, i, out var item))
                    {
                        var itemData = gameData.ManagedDataRegistry.GetOrDefault<ManagedItemData>(item.ItemType);

                        if (itemData != null)
                        {
                            if (itemData.PrefabName == prefabCoinName)
                            {
                                if (item.Stacks >= coins)
                                {
                                    InventoryUtilitiesServer.TryRemoveItemAtIndex(Plugin.Server.EntityManager, playerCharacter, item.ItemType, coins, i, false);
                                    coins = 0;
                                    break;
                                }
                                else if (item.Stacks < coins)
                                {
                                    InventoryUtilitiesServer.TryRemoveItemAtIndex(Plugin.Server.EntityManager, playerCharacter, item.ItemType, item.Stacks, i, true);
                                    coins -= item.Stacks;
                                }

                                if (coins == 0)
                                {
                                    break;
                                }

                            }
                        }
                    }
                }

                if (coins > 0)
                {
                    AdditemToIneventory(userData.CharacterName, prefabCoinGUID, price - coins);
                    return false;
                }

                return true;

            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error {error.Message}");
                return false;
            }
        }

        public static bool AdditemToIneventory(string characterName, PrefabGUID prefabGUID, int amount)
        {

            try
            {
                var user = GameData.Users.GetUserByCharacterName(characterName);
                for (int i = 0; i < amount; i++)
                {
                    var hasAdded = user.TryGiveItem(prefabGUID, 1, out Entity itemEntity);
                    if (!hasAdded)
                    {
                        user.DropItemNearby(prefabGUID, 1);
                    }
                }

                return true;

            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error {error.Message}");
                return false;
            }

        }
    }
}
