using BloodyShop.Server.Systems;
using System;
using BloodyShop.Server.Utils;
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

namespace BloodyShop.Server.Commands
{

    [Command("buy", Usage = "buy <NumberItem> <Quantity> ", Description = "Buy an object from the shop")]
    internal class Buy
    {
        private static Entity PlayerCharacter;

        private static PrefabGUID prefabCoinGUID;
        private static string prefabCoinName = string.Empty;
        private static string prefabCoinItemName = string.Empty;


        public static void Initialize(Context ctx)
        {


            BuyItem(ctx);

        }

        public static void BuyItem(Context ctx)
        {
            if (ConfigDB.getShopEnabled())
            {
                PlayerCharacter = ctx.Event.SenderCharacterEntity;

                if (ShareDB.getCoin(out ItemModel coin))
                {
                    prefabCoinGUID = new PrefabGUID(ShareDB.getCoinGUID());
                    prefabCoinName = coin.PrefabName;
                    prefabCoinItemName = coin.Name;

                    var args = ctx.Args;

                    if (args.Length < 2 || args.Length > 2)
                    {
                        Output.InvalidArguments(ctx);
                        return;
                    }

                    var indexPosition = Int32.Parse(args[0]);
                    var quantity = Int32.Parse(args[1]);

                    if (ItemsDB.SearchItem(indexPosition, out ItemShopModel itemShopModel))
                    {
                        if (itemShopModel.CheckAmountAvailability(quantity))
                        {
                            var finalPrice = itemShopModel.price * quantity;
                            if (verifyHaveSuficientCoins(ctx, finalPrice))
                            {
                                if (getCoinsFromInventory(ctx, finalPrice))
                                {
                                    if (AdditemToIneventory(ctx, itemShopModel.getPrefabGUID(), quantity))
                                    {
                                        if (ItemsDB.ModifyStock(indexPosition, quantity))
                                        {
                                            SaveDataToFiles.saveProductList();
                                            LoadDataFromFiles.loadProductList();
                                            Output.SendSystemMessage(ctx, FontColorChat.Yellow($"Transaction successful. You have purchased {FontColorChat.White($"{quantity}x {itemShopModel.getItemName()}")} for a total of  {FontColorChat.White($"{finalPrice} {prefabCoinItemName}")}"));
                                            ServerChatUtils.SendSystemMessageToAllClients(ctx.EntityManager, FontColorChat.Yellow($"{ctx.Event.User.CharacterName} has purchased {FontColorChat.White($"{quantity}x {itemShopModel.getItemName()}")} for a total of  {FontColorChat.White($"{finalPrice} {prefabCoinItemName}")}"));
                                        }
                                    }
                                    else
                                    {
                                        Plugin.Logger.LogInfo($"Error buying an item User: {ctx.Event.User.CharacterName.ToString()} Item: {itemShopModel.getItemName()} Quantity: {quantity} TotalPrice: {finalPrice}");
                                        Output.CustomErrorMessage(ctx, $"An error has occurred when delivering the items, please contact an administrator");
                                    }
                                }
                            }
                            else
                            {
                                Output.CustomErrorMessage(ctx, $"You need {FontColorChat.White($"{finalPrice} {prefabCoinItemName}")} in your inventory for this purchase");
                            };
                        }
                        else
                        {
                            Output.CustomErrorMessage(ctx, $"There is not enough stock of this item");
                        }

                    }
                    else
                    {
                        Output.CustomErrorMessage(ctx, "This item is not available in the store");
                    }
                }
                else
                {
                    Output.CustomErrorMessage(ctx, "Error loading currency type");
                }
            }
            else
            {
                Output.CustomErrorMessage(ctx, FontColorChat.Yellow($"{FontColorChat.White($"{ConfigDB.getStoreName()}")} is closed"));
            }
        }

        private static bool verifyHaveSuficientCoins(Context ctx, int coins)
        {
            try {
                NativeArray<InventoryBuffer> inventory = new NativeArray<InventoryBuffer>();
                InventoryUtilities.TryGetInventory(Plugin.Server.EntityManager, PlayerCharacter, out inventory);
                var totalCoins = InventoryUtilities.ItemCount(ref inventory, prefabCoinGUID);
                if (totalCoins >= coins)
                {
                    return true;
                } else
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

        private static bool getCoinsFromInventory(Context ctx, int price)
        {

            try
            {
                var coins = price;

                int totalSlots = InventoryUtilities.GetItemSlots(Plugin.Server.EntityManager, PlayerCharacter);

                var gameData = Plugin.Server.GetExistingSystem<GameDataSystem>();

                for (int i = 0; i < totalSlots; i++)
                {
                    if (InventoryUtilities.TryGetItemAtSlot(Plugin.Server.EntityManager, PlayerCharacter, i, out var item))
                    {
                        var itemData = gameData.ManagedDataRegistry.GetOrDefault<ManagedItemData>(item.ItemType);

                        if (itemData != null)
                        {
                            if (itemData.PrefabName == prefabCoinName)
                            {
                                if(item.Stacks >= coins)
                                {
                                    InventoryUtilitiesServer.TryRemoveItemAtIndex(Plugin.Server.EntityManager, PlayerCharacter, item.ItemType, coins, i, false);
                                    coins = 0;
                                    break;
                                } else if (item.Stacks < coins)
                                {
                                    InventoryUtilitiesServer.TryRemoveItemAtIndex(Plugin.Server.EntityManager, PlayerCharacter, item.ItemType, item.Stacks, i, true);
                                    coins -= item.Stacks;
                                }

                                if(coins == 0)
                                {
                                    break;
                                }


                            }
                        }
                    }
                }

                if (coins > 0)
                {
                    AdditemToIneventory(ctx, prefabCoinGUID, price - coins);
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

        public static bool AdditemToIneventory(Context ctx, PrefabGUID prefabGUID, int amount)
        {

            try
            {
                var user = GameData.Users.GetUserByCharacterName(ctx.Event.User.CharacterName.ToString());
                for (int i = 0; i < amount; i++)
                {
                    var hasAdded = user.TryGiveItem(prefabGUID, 1, out Entity itemEntity);
                    if (!hasAdded)
                    {
                        user.DropItemNearby(prefabGUID, 1);
                    }
                }

                return true;

            } catch (Exception error)
            {
                Plugin.Logger.LogError($"Error {error.Message}");
                return false;
            }

        }
    }
}
