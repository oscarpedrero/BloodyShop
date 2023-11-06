using VRising.GameData.Models;
using BloodyShop.DB.Models;
using BloodyShop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Bloodstone.API;
using ProjectM;
using VRising.GameData;
using BloodyShop.Client.Utils;

namespace BloodyShop.DB
{
    public class ItemsDB
    {
        public static List<PrefabModel> ProductList { get; set; } = new List<PrefabModel>();

        public static List<(string name, string type, PrefabModel model)> _normalizedItemNameCache = new();

        public static List<(string name, int GUID, string type, PrefabModel model)> _normalizedItemShopNameCache = new();

        static string _lastQueryAdd;
        static List<PrefabModel> _lastResultAdd;

        public static void generateCacheItems()
        {

            var allPrefabs = VWorld.Client.GetExistingSystem<PrefabCollectionSystem>()._PrefabGuidToEntityMap;
            foreach (var prefabEntity in allPrefabs)
            {
                var itemModel = GameData.Items.FromEntity(prefabEntity.Value);
                if (itemModel != null && itemModel?.Name != null && itemModel?.Name != "" && itemModel?.ItemType != null && itemModel?.ItemType.ToString() != "")
                {
                    var prefabModel = new PrefabModel();
                    prefabModel.PrefabName = itemModel?.Name;
                    prefabModel.PrefabType = itemModel?.ItemType.ToString();
                    prefabModel.PrefabGUID = itemModel?.Internals.PrefabGUID?.GuidHash ?? 0;
                    prefabModel.PrefabIcon = itemModel?.ManagedGameData.ManagedItemData?.Icon;
                    prefabModel.PrefabIcon = itemModel?.ManagedGameData.ManagedItemData?.Icon;

                    _normalizedItemNameCache.Add((prefabModel.PrefabName.ToString().ToLower(), prefabModel.PrefabType.ToString().ToLower(), prefabModel));
                }
            }

            //Plugin.Logger.LogInfo($"Total Prefabs = {_normalizedItemNameCache.Count}");
        }

        public static bool setProductList(List<ItemShopModel> list)
        {

            ProductList = new();
            _normalizedItemShopNameCache = new();
            foreach (var itemShopModel in list)
            {
                if (itemShopModel.isBuff)
                {
                    var prefabModel = new PrefabModel();
                    prefabModel.PrefabName = itemShopModel?.name;
                    prefabModel.PrefabType = "buff";
                    prefabModel.PrefabGUID = itemShopModel.id;
                    var icon = SpritesUtil.LoadPNGTOSprite(Properties.Resources.elixir);
                    prefabModel.PrefabIcon = icon;
                    prefabModel.PrefabPrice = itemShopModel.price;
                    prefabModel.PrefabStock = itemShopModel.stock;
                    prefabModel.PrefabStack = itemShopModel.stack;
                    prefabModel.currency = itemShopModel.currency;
                    prefabModel.isBuff = itemShopModel.isBuff;
                    ProductList.Add(prefabModel);
                    _normalizedItemShopNameCache.Add((prefabModel.PrefabName.ToString().ToLower(), prefabModel.PrefabGUID, prefabModel.PrefabType.ToString().ToLower(), prefabModel));

                } else
                {
                    var itemModel = GameData.Items.GetPrefabById(new PrefabGUID(itemShopModel.id));
                    var prefabModel = new PrefabModel();
                    prefabModel.PrefabName = itemShopModel?.name;
                    prefabModel.PrefabType = itemModel?.ItemType.ToString();
                    prefabModel.PrefabGUID = itemModel?.Internals.PrefabGUID?.GuidHash ?? 0;
                    prefabModel.PrefabIcon = itemModel?.ManagedGameData.ManagedItemData?.Icon;
                    prefabModel.PrefabPrice = itemShopModel.price;
                    prefabModel.PrefabStock = itemShopModel.stock;
                    prefabModel.PrefabStack = itemShopModel.stack;
                    prefabModel.currency = itemShopModel.currency;
                    prefabModel.isBuff = itemShopModel.isBuff;
                    ProductList.Add(prefabModel);
                    _normalizedItemShopNameCache.Add((prefabModel.PrefabName.ToString().ToLower(), prefabModel.PrefabGUID, prefabModel.PrefabType.ToString().ToLower(), prefabModel));
                }
                
            }

            //Plugin.Logger.LogInfo($"Total product List Converted {ProductList.Count}");

            return true;
        }

        public static List<PrefabModel> searchItemByNameForAdd(string text)
        {

            if (text == "")
            {
                return new List<PrefabModel>();
            }

            if (string.Equals(text, _lastQueryAdd)) return _lastResultAdd; // avoid duplicate work, idk what calls this
            _lastQueryAdd = text;

            var result = new List<PrefabModel>();
            foreach (var (name, type, model) in _normalizedItemNameCache)
            {
                if (name.Contains(text))
                {
                    result.Add(model);
                }
            }

            _lastResultAdd = result.OrderBy(x => x.PrefabType).ThenBy(x => x.PrefabName).ToList();

            return _lastResultAdd;
        }

        public static List<PrefabModel> searchItemByNameForShop(string text)
        {

            var result = new List<PrefabModel>();
            foreach (var item in _normalizedItemShopNameCache)
            {
                if (item.name.Contains(text))
                {
                    result.Add(item.model);
                }
            }

            return result;
        }

        public static int searchIndexForProduct(int GUID, int stack, CurrencyModel currency)
        {
            var index = 1;
            foreach (var item in ProductList)
            {
                if (item.PrefabGUID == GUID && item.PrefabStack == stack && item.currency == currency.guid)
                {
                    return index;
                }
                index++;
            }

            return -1;
        }

        public static List<ItemShopModel> getProductListForSaveJSON()
        {

            var productListReturn = new List<ItemShopModel>();
            foreach (var prefabModel in ProductList)
            {
                var itemShopModel = new ItemShopModel();
                itemShopModel.id = prefabModel.PrefabGUID;
                itemShopModel.name = prefabModel.PrefabName;
                itemShopModel.stock = prefabModel.PrefabStock;
                itemShopModel.stack = prefabModel.PrefabStack;
                itemShopModel.currency = prefabModel.currency;
                itemShopModel.price = prefabModel.PrefabPrice;
                itemShopModel.isBuff = prefabModel.isBuff;
                productListReturn.Add(itemShopModel);
            }

            return productListReturn;
        }

        public static bool addProductList(int item, int price, int stock, string name, int currency, int stack = 1, bool isBuff = false)
        {

            if(isBuff)
            {
                PrefabModel prefabModel = new PrefabModel();
                prefabModel.PrefabName = name;
                prefabModel.PrefabType = "buff";
                prefabModel.PrefabGUID = item;
                var icon = SpritesUtil.LoadPNGTOSprite(Properties.Resources.elixir);
                prefabModel.PrefabIcon = icon;
                prefabModel.PrefabPrice = price;
                prefabModel.PrefabStock = stock;
                prefabModel.PrefabStack = stack;
                prefabModel.currency = currency;
                prefabModel.isBuff = true;

                ProductList.Add(prefabModel);
                _normalizedItemShopNameCache.Add((prefabModel.PrefabName.ToString().ToLower(), prefabModel.PrefabGUID, prefabModel.PrefabType.ToString().ToLower(), prefabModel));
                return true;

            } else
            {
                var itemModel = GameData.Items.GetPrefabById(new PrefabGUID(item));
                if (itemModel == null) return false;

                PrefabModel prefabModel = new PrefabModel();
                prefabModel.PrefabName = name;
                prefabModel.PrefabType = itemModel?.ItemType.ToString();
                prefabModel.PrefabGUID = itemModel?.Internals.PrefabGUID?.GuidHash ?? 0;
                prefabModel.PrefabIcon = itemModel?.ManagedGameData.ManagedItemData?.Icon;
                prefabModel.PrefabPrice = price;
                prefabModel.PrefabStock = stock;
                prefabModel.PrefabStack = stack;
                prefabModel.currency = currency;
                prefabModel.isBuff = isBuff;

                ProductList.Add(prefabModel);
                _normalizedItemShopNameCache.Add((prefabModel.PrefabName.ToString().ToLower(), prefabModel.PrefabGUID, prefabModel.PrefabType.ToString().ToLower(), prefabModel));
                return true;
            }
            

        }

        public static bool RemoveItemByCommand(int index)
        {
            try
            {
                ProductList.RemoveAt(index - 1);
                _normalizedItemShopNameCache.RemoveAt(index - 1);
                return true;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }

        }

        public static bool SearchItemByCommand(int index, out PrefabModel itemShopModel)
        {

            try
            {
                if (ProductList.Count >= index)
                {
                    itemShopModel = ProductList[index - 1];
                    return true;
                }
                else
                {
                    itemShopModel = null;
                    return false;
                }
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                itemShopModel = null;
                return false;
            }
        }

        public static bool SearchProductListAndRemoveItemByCommmand(int index, out PrefabModel itemShopModel)
        {

            try
            {
                if (ProductList.Count >= index)
                {
                    itemShopModel = ProductList[index - 1];
                    if (RemoveItemByCommand(index))
                    {
                        return true;
                    }
                    else
                    {
                        itemShopModel = null;
                        return false;
                    }
                }
                else
                {
                    itemShopModel = null;
                    return false;
                }
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                itemShopModel = null;
                return false;
            }

        }

        public static bool ModifyStockByCommand(int index, int amount)
        {

            try
            {
                if (ProductList.Count < index)
                {
                    return false;
                }
                else
                {
                    var itemShopModel = ProductList[index - 1];
                    var actualStock = itemShopModel.PrefabStock;
                    if (actualStock < 0) return true;
                    itemShopModel.PrefabStock = actualStock - amount;
                    if (itemShopModel.PrefabStock == 0)
                    {
                        RemoveItemByCommand(index);
                    }
                    else
                    {
                        ProductList[index - 1] = itemShopModel;
                        _normalizedItemShopNameCache[index - 1] = (itemShopModel.PrefabName.ToString().ToLower(), itemShopModel.PrefabGUID, itemShopModel.PrefabType.ToString().ToLower(), itemShopModel);
                    }
                    return true;
                }
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message} {error.Source}");
                return false;
            }


        }

        public static bool SearchProductListAndRemoveItem(int itemGUID, out PrefabModel itemShopModel)
        {

            itemShopModel = null;

            int index = 0;
            foreach (var item in _normalizedItemShopNameCache)
            {
                if (item.GUID == itemGUID)
                {
                    _normalizedItemShopNameCache.Remove(item);
                    ProductList.Remove(item.model);
                    itemShopModel = item.model;
                    break;
                }
                index++;
            }

            if (itemShopModel == null) return false;

            return true;
           
        }

        public static bool SearchProductList(int itemGUID, out PrefabModel itemShopModel)
        {

            itemShopModel = null;

            int index = 0;
            foreach (var item in _normalizedItemShopNameCache)
            {
                if (item.GUID == itemGUID)
                {
                    _normalizedItemShopNameCache.Remove(item);
                    ProductList.Remove(item.model);
                    itemShopModel = item.model;
                    break;
                }
                index++;
            }

            if (itemShopModel == null) return false;

            return true;
           
        }

        public static bool SearchProductsWithCurrencyByCommand(CurrencyModel currencyModel, out int itemShopModel)
        {

            itemShopModel = 0;

            foreach (var item in ProductList)
            {
                if (item.currency == currencyModel.guid)
                {
                    itemShopModel++;
                }
            }

            if (itemShopModel == 0) return false;

            return true;
           
        }

        public static List<string> GetProductListMessage()
        {
            var listItems = new List<string>();

            int index = 1;

            var prefabName = ""; 
            
            
                foreach (PrefabModel item in ProductList)
                {
                    var currency = ShareDB.getCurrency(item.currency);

                    if (currency != null)
                    {
                        // STOCK ITEM
                        var finalStock = "";
                        if (item.PrefabStock <= 0)
                        {
                            finalStock = "Infinite";
                            prefabName = "1x " + item.PrefabName;
                        }
                        else
                        {
                            finalStock = item.PrefabStock.ToString();
                            prefabName = item.PrefabStack.ToString()+"x "+item.PrefabName;
                        }
                        listItems.Add($"{FontColorChat.White("[")}{FontColorChat.Yellow(index.ToString())}{FontColorChat.White("]")} " +
                            $"{FontColorChat.Yellow(prefabName)} " +
                            $"{FontColorChat.Red("Price:")} {FontColorChat.Yellow(item.PrefabPrice.ToString())} {FontColorChat.White($"{currency.name}")} " +
                            $"{FontColorChat.Red("Stock:")} {FontColorChat.Yellow(finalStock)} units");
                        index++;
                    }
                }

            return listItems;

        }

    }
}
