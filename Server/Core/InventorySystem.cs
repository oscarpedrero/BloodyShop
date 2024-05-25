using System;
using Unity.Entities;
using Unity.Collections;
using ProjectM;
using Bloody.Core;
using Bloody.Core.GameData.v1;
using Bloody.Core.Methods;
using Stunlock.Core;

namespace BloodyShop.Server.Core
{
    public  class InventorySystem
    {
        public static bool searchPrefabsInInventory(string characterName, PrefabGUID prefabCurrencyGUID, out int total)
        {
            total = 0;
            try
            {

                var userData = GameData.Users.GetUserByCharacterName(characterName);
                var characterEntity = userData.Character.Entity;

                NativeArray<InventoryBuffer> inventory = new NativeArray<InventoryBuffer>();
                InventoryUtilities.TryGetInventory(Plugin.Server.EntityManager, characterEntity, out inventory);

                total = InventoryUtilities.GetItemAmount(Plugin.Server.EntityManager, characterEntity, prefabCurrencyGUID);
                if (total >= 0)
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
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }

        }
        public static bool verifyHaveSuficientPrefabsInInventory(string characterName, PrefabGUID prefabCurrencyGUID, int quantity = 1)
        {
            try
            {

                if (searchPrefabsInInventory(characterName, prefabCurrencyGUID, out int total))
                {
                    if (total >= quantity)
                    {
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
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }

        }

        public static bool getPrefabFromInventory(string characterName, PrefabGUID prefabGUID, int quantity)
        {

            try
            {

                var totalItemsRemove = quantity;

                var userData = GameData.Users.GetUserByCharacterName(characterName);

                var prefabGameData = GameData.Items.GetPrefabById(prefabGUID);

                var userEntity = userData.Character.Entity;

                int totalSlots = InventoryUtilities.GetInventorySize(Plugin.Server.EntityManager, userEntity);

                var gameDataSystem = Plugin.Server.GetExistingSystemManaged<GameDataSystem>();

                for (int i = 0; i < totalSlots; i++)
                {

                    if (InventoryUtilities.TryGetItemAtSlot(Plugin.Server.EntityManager, userEntity, i, out var item))
                    {
                        //var itemData = gameDataSystem.ManagedDataRegistry.GetOrDefault<ManagedItemData>(item.ItemType);

                        var itemData = GameData.Items.GetPrefabById(item.ItemType);

                        if (itemData != null)
                        {
                            if (itemData.PrefabName == prefabGameData.PrefabName)
                            {
                                if (item.Amount >= totalItemsRemove)
                                {
                                    InventoryUtilitiesServer.TryRemoveItemAtIndex(Plugin.Server.EntityManager, userEntity, item.ItemType, totalItemsRemove, i, false);
                                    totalItemsRemove = 0;
                                    break;
                                }
                                else if (item.Amount < totalItemsRemove)
                                {
                                    InventoryUtilitiesServer.TryRemoveItemAtIndex(Plugin.Server.EntityManager, userEntity, item.ItemType, item.Amount, i, true);
                                    totalItemsRemove -= item.Amount;
                                }

                                if (totalItemsRemove == 0)
                                {
                                    break;
                                }

                            }
                        }
                    }
                }

                if (totalItemsRemove > 0)
                {
                    AdditemToInventory(userData.CharacterName, prefabGUID, quantity - totalItemsRemove);
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

        public static bool AdditemToInventory(string characterName, PrefabGUID prefabGUID, int quantity)
        {

            try
            {

                var user = GameData.Users.GetUserByCharacterName(characterName);

                for (int i = 0; i < quantity; i++)
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
