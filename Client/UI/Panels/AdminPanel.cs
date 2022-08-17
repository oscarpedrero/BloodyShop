using BloodyShop.DB;
using VRising.GameData.Models;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using BloodyShop.Client.DB;
using UnityEngine.EventSystems;
using BloodyShop.Network.Messages;
using BloodyShop.Client.Network;

namespace BloodyShop.Client.UI.Panels
{
    public class AdminPanel : UniverseLib.UI.Panels.PanelBase
    {

        public static AdminPanel Instance { get; private set; }

        public override string Name => "Admin Tool";

        public override bool CanDragAndResize => false;

        public override int MinWidth => 600;
        public override int MinHeight => (ItemsDB.GetProductList().Count * 30)+100;
        public override Vector2 DefaultAnchorMin => new(600, 420);
        public override Vector2 DefaultAnchorMax => new(600, 420);
        public override Vector2 DefaultPosition => new((Screen.height / 2) - 600, (Screen.width / 2) - 420);

        public GameObject NavbarHolder;
        public Dropdown MouseInspectDropdown;
        public GameObject ContentHolder;
        public GameObject ContentProduct;
        public RectTransform ContentRect;
        public Text resultTxt;

        public static float CurrentPanelWidth => Instance.Rect.rect.width;
        public static float CurrentPanelHeight => Instance.Rect.rect.height;

        public int CurrentDisplayedIndex;

        int itemNewAdd = 0;

        public static bool active = false;

        InputFieldRef quantityNew;

        InputFieldRef priceNew;

        private static string[] itemsGame { get; set; }

    public AdminPanel(UIBase owner) : base(owner)
        {
            Instance = this;
        }

        public override void Update()
        {
            //InspectorManager.Update();
        }

        public override void OnFinishResize()
        {
            base.OnFinishResize();
        }

        protected override void ConstructPanelContent()
        {

            itemsGame = ClientDB.allItemsGame;

            active = true;

            // TitleBar
            GameObject closeHolder = this.TitleBar.transform.Find("CloseHolder").gameObject;

            //INSERT LAYOUT
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(this.ContentRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

            // NAV BAR
            this.NavbarHolder = UIFactory.CreateGridGroup(this.ContentRoot, "Navbar", new Vector2(200, 22), new Vector2(4, 4),
                new Color(0.05f, 0.05f, 0.05f));
            UIFactory.SetLayoutElement(NavbarHolder, flexibleWidth: 9999, minHeight: 0, preferredHeight: 0, flexibleHeight: 9999);
            NavbarHolder.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            // OPEN BTN
            ButtonRef openStoreBtn = UIFactory.CreateButton(NavbarHolder, "OpenStoreBtn", "Open Atore", new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(openStoreBtn.Component.gameObject, minHeight: 25, minWidth: MinWidth / 3);
            //openStoreBtn.Component.transform.SetSiblingIndex(openStoreBtn.Component.transform.GetSiblingIndex() - 1);
            openStoreBtn.OnClick += OpenAction;

            // CLOSE BTN
            ButtonRef closeStoreBtn = UIFactory.CreateButton(NavbarHolder, "CloseStoreBtn", "Close Store", new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(closeStoreBtn.Component.gameObject, minHeight: 25, minWidth: MinWidth / 3);
            //closeStoreBtn.Component.transform.SetSiblingIndex(closeStoreBtn.Component.transform.GetSiblingIndex() - 2);
            closeStoreBtn.OnClick += CloseAction;

            // REFRESH BTN
            ButtonRef refreshBtn = UIFactory.CreateButton(closeHolder.gameObject, "RefreshBtn", "Refresh",
                new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(refreshBtn.Component.gameObject, minHeight: 25, minWidth: MinWidth / 3);
            //refreshBtn.Component.transform.SetSiblingIndex(refreshBtn.Component.transform.GetSiblingIndex() - 3);
            refreshBtn.OnClick += RefreshAction;

            var items = ItemsDB.GetProductList();

            var index = 1;
            foreach (var item in items)
            {
                if (ShareDB.getCoin(out ItemModel coin))
                {

                    // CONTAINER FOR PRODUCTS
                    var _contentProduct = UIFactory.CreateHorizontalGroup(this.ContentRoot, "ContentItem-" + index, true, true, true, true, 0, default, new Color(0.1f, 0.1f, 0.1f));

                    //NUMBER ITEM
                    Text indexName = UIFactory.CreateLabel(_contentProduct, "indexTxt-" + index, $"{index}", TextAnchor.MiddleCenter);
                    UIFactory.SetLayoutElement(indexName.gameObject, minWidth: 10, minHeight: 25);

                    //NAME ITEM
                    Text itemName = UIFactory.CreateLabel(_contentProduct, "itemNameTxt-" + index, $"{item.getItemName()}", TextAnchor.MiddleCenter);
                    UIFactory.SetLayoutElement(itemName.gameObject, minWidth: 200, minHeight: 25);

                    // PRICE ITEM
                    Text itemPrice = UIFactory.CreateLabel(_contentProduct, "itemPriceTxt-" + index, $"{item.price} {coin.Name}", TextAnchor.MiddleCenter);
                    UIFactory.SetLayoutElement(itemPrice.gameObject, minWidth: 100, minHeight: 25);

                    // Quantity ITEM
                    Text itemQuantity = UIFactory.CreateLabel(_contentProduct, "itemQuantityTxt-" + index, $"{item.amount}", TextAnchor.MiddleCenter);
                    UIFactory.SetLayoutElement(itemPrice.gameObject, minWidth: 10, minHeight: 25);

                    // DELETE BTN
                    ButtonRef deleteBtn = UIFactory.CreateButton(_contentProduct, "deleteItemBtn-" + index, "Detele", new Color(0.3f, 0.2f, 0.2f));
                    UIFactory.SetLayoutElement(deleteBtn.Component.gameObject, minHeight: 25, minWidth: 80);
                    deleteBtn.OnClick += DeleteAction;


                    UIFactory.SetLayoutElement(_contentProduct, flexibleHeight: 9999);
                    index++;
                }
            }

            this.NavbarHolder = UIFactory.CreateGridGroup(this.ContentRoot, "Navbar", new Vector2(200, 22), new Vector2(4, 4),
                new Color(0.05f, 0.05f, 0.05f));
            UIFactory.SetLayoutElement(NavbarHolder, flexibleWidth: 9999, minHeight: 25, preferredHeight: 25, flexibleHeight: 9999);
            NavbarHolder.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var headLabel = UIFactory.CreateLabel(NavbarHolder, "resultTXT", $"Add Item to Store", TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(headLabel.gameObject, minWidth: 150, minHeight: 50);

            var _contentNewProduct = UIFactory.CreateHorizontalGroup(this.ContentRoot, "ContentAdd", true, true, true, true, 0, default, new Color(0.1f, 0.1f, 0.1f));

            GameObject dropDown = UIFactory.CreateDropdown(
                        _contentNewProduct,
                        "Item",
                        out var dropdown,
                        "Select a Item",
                        14,
                        ItemDropdownValueChanged,
                        itemsGame);

            UIFactory.SetLayoutElement(dropDown, minWidth: 250, minHeight: 25);

            // PRICE ITEM
            priceNew = UIFactory.CreateInputField(_contentNewProduct, "priceNew", "Price");
            UIFactory.SetLayoutElement(priceNew.GameObject, minWidth: 15, minHeight: 25);
            priceNew.Component.contentType = InputField.ContentType.IntegerNumber;

            // Quantity ITEM
            quantityNew = UIFactory.CreateInputField(_contentNewProduct, "quantityNew", "Quantity");
            UIFactory.SetLayoutElement(quantityNew.GameObject, minWidth: 15, minHeight: 25);
            priceNew.Component.contentType = InputField.ContentType.IntegerNumber;

            // SAVE BTN
            ButtonRef saveBtn = UIFactory.CreateButton(_contentNewProduct, "saveBtn", "Add Item", new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(saveBtn.Component.gameObject, minHeight: 25, minWidth: 80);
            saveBtn.OnClick += SaveAction;


            this.SetActive(true);



        }

        private void ItemDropdownValueChanged(int value)
        {
            var nameArray = itemsGame[value].Split("| ");
            itemNewAdd = Int32.Parse(nameArray[1]);
        }

        private void SaveAction()
        {
            var item = itemNewAdd;
            var price = priceNew.Text;
            var quantity = quantityNew.Text;
            if(price != "" || quantity != "")
            {
                var msg = new AddSerializedMessage()
                {
                    PrefabGUID = item.ToString(),
                    Price = price,
                    Quantity = quantity,
                };
                ClientAddMessageAction.Send(msg);
                RefreshAction();
            }
        }


        private void DeleteAction()
        {
            resultTxt.text = "";
            var btnName = EventSystem.current.currentSelectedGameObject.name;
            var indexToDelete = btnName.Replace("deleteItemBtn-", "");
            var msg = new DeleteSerializedMessage()
            {
                Item = indexToDelete
            };
            ClientDeleteMessageAction.Send(msg);
            RefreshAction();
            resultTxt.text = "Delete item successfully";

            Plugin.Logger.LogInfo(indexToDelete);
        }

        private void RefreshAction()
        {
            UIManager.removePanel();
        }


        private void OpenAction()
        {
            ClientOpenMessageAction.Send();
        }


        private void CloseAction()
        {
            ClientCloseMessageAction.Send();
        }

        // override other methods as desired
    }
}
