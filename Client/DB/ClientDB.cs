using BloodyShop.Client.DB.Models;
using ProjectM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VRising.GameData;
using VRising.GameData.Models;
using Wetstone.API;

namespace BloodyShop.Client.DB
{
    public class ClientDB
    {
        public static bool serverHaveMod { get; set; } = false;
        public static bool gameDataInit { get; set; } = false;
        public static string shopName { get; set; }
        public static bool shopOpen { get; set; } = true;
        public static string prefix { get; set; }
        public static UserModel userModel { get; set; }

        public static List<(string name, PrefabModel model)> _normalizedItemNameCache = new();
        public static List<ItemModel> itemsAdd { get; set; } = new List<ItemModel>();

        static string _lastQuery;

        public static void generateCacheItems()
        {

            var allPrefabs = VWorld.Client.GetExistingSystem<PrefabCollectionSystem>().PrefabLookupMap;
            foreach (var prefabEntity in allPrefabs)
            {
                var itemModel = GameData.Items.FromEntity(prefabEntity.Value);
                if (itemModel != null && itemModel?.Name != null)
                {
                    var prefabModel = new PrefabModel();
                    prefabModel.PrefabName = itemModel?.Name;
                    prefabModel.PrefabType = itemModel?.ItemType.ToString();
                    prefabModel.PrefabGUID = itemModel?.Internals.PrefabGUID?.GuidHash;
                    prefabModel.PrefabIcon = itemModel?.ManagedGameData.ManagedItemData?.Icon;

                    var key = prefabModel.PrefabName.ToString().ToLower();
                    _normalizedItemNameCache.Add((key, prefabModel));
                }
            }

            Plugin.Logger.LogInfo($"Total Prefabs = {_normalizedItemNameCache.Count}");
        }

        public static List<PrefabModel> searchItemByName(string text)
        {
            if(text == "")
            {
                return null;
            }

            if (string.Equals(text, _lastQuery)) return null; // avoid duplicate work, idk what calls this
            _lastQuery = text;
            var sw = new Stopwatch();
            var result = new List<PrefabModel>();
            foreach (var item in _normalizedItemNameCache)
            {
                if (item.name.Contains(text.ToString()))
                {
                    result.Add(item.model);
                }
            }

            return result;
        }

        /*public static List<ItemModel> searchItemByName(string text)
        {

            var itemsFind = from item in itemModels
                            where item.Name != null && item.Name.ToString().ToLower().Contains(text.ToString().ToLower())
                            orderby item.ItemType.ToString() ascending, item.Name.ToString() ascending
                            select item;

            return itemsFind.ToList();

        }*/

    }
}