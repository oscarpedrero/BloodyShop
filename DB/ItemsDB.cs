using VRising.GameData.Models;
using BloodyShop.DB.Models;
using BloodyShop.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BloodyShop.DB
{
    public class ItemsDB
    {
        private static List<ItemShopModel> ProductList { get; set; } = new List<ItemShopModel>();

        public static bool setProductList(List<ItemShopModel> list)
        {
            ProductList = list;
            ProductList.Reverse();
            return true;
        }

        public static void addProductList(int item, int price, int amount)
        {
            ItemShopModel itemNew = new ItemShopModel()
            {
                id = item,
                price = price,
                amount = amount
            };
            ProductList.Add(itemNew);
        }

        public static bool SearchItem(int item, out ItemShopModel itemShopModel)
        {
            try
            {
                if (ProductList.Count >= item)
                {
                    itemShopModel = ProductList[item - 1];
                    return true;
                } else
                {
                    itemShopModel = null;
                    return false;
                }
               
            } catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                itemShopModel = null;
                return false;
            }
        }

        public static List<ItemShopModel> GetProductList()
        {
            return ProductList;
        }

        public static List<string> GetProductListMessage()
        {
            var listItems = new List<string>();

            int index = 1;

            if(ShareDB.getCoin(out ItemModel coin))
            {
                foreach (ItemShopModel item in ProductList)
                {
                    listItems.Add($"{FontColorChat.White("[")}{FontColorChat.Yellow(index.ToString())}{FontColorChat.White("]")} " +
                        $"{FontColorChat.Yellow(item.getItemName())} " +
                        $"{FontColorChat.Red("Price:")} {FontColorChat.Yellow(item.price.ToString())} {FontColorChat.White($"{coin?.Name.ToString()}")} " +
                        $"{FontColorChat.Red("Available:")} {FontColorChat.Yellow(item.amount.ToString())} units");
                    index++;
                }
            }

            return listItems;

        }

        public static bool ModifyStock(int index, int amount)
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
                    var actualStock = itemShopModel.amount;
                    itemShopModel.amount = actualStock - amount;
                    if (itemShopModel.amount == 0)
                    {
                        RemoveItem(index);
                    }
                    else
                    {
                        ProductList[index - 1]= itemShopModel;
                    }
                    return true;
                }
            } catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }
            

        }

        public static bool RemoveItem(int index)
        {
            try
            {
                ProductList.RemoveAt(index - 1);
                return true;
            } catch (Exception error)
            {
                Plugin.Logger.LogError($"Error: {error.Message}");
                return false;
            }
            
        }
    }
}
