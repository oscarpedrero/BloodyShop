using ProjectM;

namespace BloodyShop.DB.Models
{
    public class ItemShopModelOLD
    {
        private static PrefabGUID prefabGUID;

        private static int priceItem;

        public int getPrice()
        {
            return priceItem;
        }

        public PrefabGUID getPrefabGUID()
        {
            return prefabGUID;
        }

        public string getItemName() {
            var ItemDataDropGroupEntity = Plugin.Server.GetExistingSystem<PrefabCollectionSystem>().PrefabLookupMap[prefabGUID];
            var itemData = Plugin.Server.EntityManager.GetComponentData<ProjectM.ItemData>(ItemDataDropGroupEntity);
            var managedDataRegistry = Plugin.Server.GetExistingSystem<GameDataSystem>().ManagedDataRegistry;
            var managedItemData = managedDataRegistry.GetOrDefault<ManagedItemData>(itemData.ItemTypeGUID);
            return managedItemData?.Name.ToString();
        }

        public string getPrefabName()
        {
            return Plugin.Server.GetExistingSystem<PrefabCollectionSystem>().PrefabNameLookupMap[prefabGUID].ToString();
        }

        public void setPrice(int price)
        {
            priceItem = price;
        }

        public void setPrefabGUID(int guid)
        {
            prefabGUID = new PrefabGUID(guid);
        }
        
    }
}
