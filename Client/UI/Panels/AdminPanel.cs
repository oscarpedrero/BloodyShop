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
using UniverseLib.UI.Widgets;
using UniverseLib;
using System.Collections.Generic;

namespace BloodyShop.Client.UI.Panels
{
    public class AdminPanel : UniverseLib.UI.Panels.PanelBase
    {

        public static AdminPanel Instance { get; private set; }

        public override string Name => "Admin Shop";

        public override bool CanDragAndResize => true;

        public override int MinWidth => 680;
        public override int MinHeight => 535;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 1f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 1f);
        public override Vector2 DefaultPosition => new Vector2(0 - (MinWidth / 2) - 680, 0 + (MinWidth / 2));

        public GameObject NavbarHolder;
        public Dropdown MouseInspectDropdown;
        public GameObject ContentHolder;
        public GameObject ContentProduct;
        public RectTransform ContentRect;
        public Text alertTXT;
        public GameObject contentScroll;
        public Dropdown dropdowntype;
        public Dropdown dropdownitem;
        public ButtonRef OpenCloseStoreBtn;
        public List<GameObject> productsListLayers = new List<GameObject>();

        public static float CurrentPanelWidth => Instance.Rect.rect.width;
        public static float CurrentPanelHeight => Instance.Rect.rect.height;

        public int CurrentDisplayedIndex;

        int itemNewAdd = 0;

        public static bool active = false;

        InputFieldRef quantityNew;

        InputFieldRef priceNew;

        private static string[] itemsGame { get; set; }
        private static string[] itemsType { get; set; }

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

            active = true;

            // TITLE Bar
            GameObject closeHolder = this.TitleBar.transform.Find("CloseHolder").gameObject;

            // REFRESH BTN
            ButtonRef refreshBtn = UIFactory.CreateButton(closeHolder.gameObject, "RefreshBtn", "Refresh",
                new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(refreshBtn.Component.gameObject, minHeight: 25, minWidth: 180);
            refreshBtn.Component.transform.SetSiblingIndex(refreshBtn.Component.transform.GetSiblingIndex() - 1);
            refreshBtn.OnClick += RefreshAction;
            RuntimeHelper.SetColorBlock(refreshBtn.Component,
                       new Color(31 / 255f, 97 / 255f, 141 / 255f),
                        new Color(36 / 255f, 113 / 255f, 163 / 255f),
                        new Color(41 / 255f, 128 / 255f, 185 / 255f));

            var textBTNOpenClose = "Open Store";

            // OPEN / CLOSE BTN
            OpenCloseStoreBtn = UIFactory.CreateButton(closeHolder.gameObject, "CloseStoreBtn", textBTNOpenClose, new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(OpenCloseStoreBtn.Component.gameObject, minHeight: 25, minWidth: 180);
            OpenCloseStoreBtn.Component.transform.SetSiblingIndex(OpenCloseStoreBtn.Component.transform.GetSiblingIndex() - 2);
            if (ClientDB.shopOpen)
            {
                OpenCloseStoreBtn.Component.GetComponentInChildren<Text>().text = "Close Store";
                RuntimeHelper.SetColorBlock(OpenCloseStoreBtn.Component,
                       new Color(33 / 255f, 47 / 255f, 60 / 255f),
                        new Color(40 / 255f, 55 / 255f, 71 / 255f),
                        new Color(93 / 255f, 109 / 255f, 126 / 255f));
            }
            else
            {
                OpenCloseStoreBtn.Component.GetComponentInChildren<Text>().text = "Open Store";
                RuntimeHelper.SetColorBlock(OpenCloseStoreBtn.Component,
                      new Color(17 / 255f, 122 / 255f, 101 / 255f),
                       new Color(19 / 255f, 141 / 255f, 117 / 255f),
                       new Color(22 / 255f, 160 / 255f, 133 / 255f));
            }
            OpenCloseStoreBtn.OnClick += OpenCloseAction;



            //INSERT LAYOUT
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(this.ContentRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

            // CONTAINER FOR PRODUCTS
            var _contentHeader = UIFactory.CreateHorizontalGroup(this.ContentRoot.gameObject, "HeaderItem", true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

            // Aval ITEM
            Text headerAval = UIFactory.CreateLabel(_contentHeader, "itemAvalTxt", $"Stock");
            UIFactory.SetLayoutElement(headerAval.gameObject, minWidth: 50, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 50);

            // ITEM ICON
            Text headerName = UIFactory.CreateLabel(_contentHeader, "itemNameTxt", $"Name", TextAnchor.MiddleLeft);

            UIFactory.SetLayoutElement(headerName.gameObject, minWidth: 60, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 60);

            //NAME ITEM
            var imageHeader = UIFactory.CreateUIObject("IconItem", _contentHeader);
            UIFactory.SetLayoutElement(imageHeader, minWidth: 310, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 310);

            // PRICE ITEM
            Text headerPrice = UIFactory.CreateLabel(_contentHeader, "itemPriceTxt", $"Price");
            UIFactory.SetLayoutElement(headerPrice.gameObject, minWidth: 100, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 100);

            // BUY BTN
            var headerBuy = UIFactory.CreateUIObject("BuyItem", _contentHeader);
            UIFactory.SetLayoutElement(headerBuy, minWidth: 110, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 110);

            UIFactory.SetLayoutElement(_contentHeader, flexibleHeight: 0, minHeight: 60, preferredHeight: 60, flexibleWidth: 0);

            contentScroll = new GameObject();

            var _scroolView = UIFactory.CreateScrollView(this.ContentRoot.gameObject, "scrollView", out contentScroll, out AutoSliderScrollbar autoSliderScrollbar);

            CreateListProductsLayou();



            this.NavbarHolder = UIFactory.CreateGridGroup(this.ContentRoot, "Navbar", new Vector2(200, 22), new Vector2(4, 4),
                new Color(0.05f, 0.05f, 0.05f));
            UIFactory.SetLayoutElement(NavbarHolder, flexibleWidth: 9999, minHeight: 22, preferredHeight: 22, flexibleHeight: 0);
            NavbarHolder.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var headLabel = UIFactory.CreateLabel(NavbarHolder, "resultTXT", $"Add Item to Store", TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(headLabel.gameObject, minWidth: MinWidth, minHeight: 50);

            var _contentHeaderFormNewProduct = UIFactory.CreateHorizontalGroup(this.ContentRoot, "ContentAdd", true, true, true, true, 0, default, new Color(0.1f, 0.1f, 0.1f));

            // TYPE DROPDOWN LABEL
            Text headerTypeLabel = UIFactory.CreateLabel(_contentHeaderFormNewProduct, "headerTypeDropDownLabel", $"Item Type");
            UIFactory.SetLayoutElement(headerTypeLabel.gameObject, minWidth: 150, minHeight: 25, preferredWidth: 150, preferredHeight: 25, flexibleHeight: 0, flexibleWidth: 0);

            // ITEM DROPDOWN LABEL
            Text headerItenLabel = UIFactory.CreateLabel(_contentHeaderFormNewProduct, "headerItemDropDownLabel", $"Item");
            UIFactory.SetLayoutElement(headerItenLabel.gameObject, minWidth: 260, minHeight: 25, preferredWidth: 280, preferredHeight: 25, flexibleHeight: 0, flexibleWidth: 0);

            // PRICE INPUT LABEL
            Text headerPriceLabel = UIFactory.CreateLabel(_contentHeaderFormNewProduct, "headerPriceInputLabel", $"Price");
            UIFactory.SetLayoutElement(headerPriceLabel.gameObject, minWidth: 40, minHeight: 25, preferredWidth: 40, preferredHeight: 25, flexibleHeight: 0, flexibleWidth: 0);

            // STOCK INPUT LABEL
            Text headerStockLabel = UIFactory.CreateLabel(_contentHeaderFormNewProduct, "headerPriceInputLabel", $"Stock");
            UIFactory.SetLayoutElement(headerStockLabel.gameObject, minWidth: 40, minHeight: 25, preferredWidth: 40, preferredHeight: 25, flexibleHeight: 0, flexibleWidth: 0);

            // BUTTON LABEL
            Text headerButtonLabel = UIFactory.CreateLabel(_contentHeaderFormNewProduct, "headerButtontLabel", $"");
            UIFactory.SetLayoutElement(headerButtonLabel.gameObject, minWidth: 80, minHeight: 25, preferredWidth: 80, preferredHeight: 25, flexibleHeight: 0, flexibleWidth: 0);

            var _contentNewProduct = UIFactory.CreateHorizontalGroup(this.ContentRoot, "ContentAdd", true, true, true, true, 0, default, new Color(0.1f, 0.1f, 0.1f));

            itemsType = ClientDB.getAllTypes();

            GameObject dropDownType = UIFactory.CreateDropdown(
                        _contentNewProduct,
                        "Item",
                        out dropdowntype,
                        "Select a type",
                        14,
                        TypeDropdownValueChanged,
                        itemsType);

            UIFactory.SetLayoutElement(dropDownType, minWidth: 150, minHeight: 50, preferredWidth: 150, preferredHeight: 50, flexibleHeight: 0, flexibleWidth: 0);

            GameObject dropDownItem = UIFactory.CreateDropdown(
                        _contentNewProduct,
                        "Item",
                        out dropdownitem,
                        "Select a Item",
                        14,
                        ItemDropdownValueChanged,
                        itemsGame);

            UIFactory.SetLayoutElement(dropDownItem, minWidth: 260, minHeight: 50, preferredWidth: 280, preferredHeight: 50, flexibleHeight: 0, flexibleWidth: 0);

            // PRICE ITEM
            priceNew = UIFactory.CreateInputField(_contentNewProduct, "priceNew", "Price");
            UIFactory.SetLayoutElement(priceNew.GameObject, minWidth: 40, minHeight: 50, preferredWidth: 40, preferredHeight: 50, flexibleHeight: 0, flexibleWidth: 0);
            priceNew.Component.contentType = InputField.ContentType.IntegerNumber;

            // Quantity ITEM
            quantityNew = UIFactory.CreateInputField(_contentNewProduct, "quantityNew", "Stock");
            UIFactory.SetLayoutElement(quantityNew.GameObject, minWidth: 40, minHeight: 50, preferredWidth: 40, preferredHeight: 50, flexibleHeight: 0, flexibleWidth: 0);
            priceNew.Component.contentType = InputField.ContentType.IntegerNumber;

            // SAVE BTN
            ButtonRef saveBtn = UIFactory.CreateButton(_contentNewProduct, "saveBtn", "Add Item", new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(saveBtn.Component.gameObject, minWidth: 80, minHeight: 50, preferredWidth: 80, preferredHeight: 50, flexibleHeight: 0, flexibleWidth: 0);
            saveBtn.OnClick += SaveAction;


            this.SetActive(true);

        }

        public void CreateListProductsLayou()
        {

            UnityEngine.Object.Destroy(alertTXT, 0.2f);
            var items = ItemsDB.GetProductList();

            var index = items.Count;
            productsListLayers = new List<GameObject>();
            foreach (var item in items)
            {
                if (ShareDB.getCoin(out ItemModel coin))
                {
                    // CONTAINER FOR PRODUCTS
                    var _contentProduct = UIFactory.CreateHorizontalGroup(contentScroll, "ContentItem-" + index, true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

                    // Aval ITEM
                    Text itemAval = UIFactory.CreateLabel(_contentProduct, "itemAvalTxt-" + index, $"{item.amount}", TextAnchor.MiddleCenter);
                    UIFactory.SetLayoutElement(itemAval.gameObject, minWidth: 50, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 50);

                    // ITEM ICON
                    var imageIcon = UIFactory.CreateUIObject("IconItem-" + index, _contentProduct);
                    var iconImage = imageIcon.AddComponent<Image>();
                    iconImage.sprite = item.getIcon();
                    UIFactory.SetLayoutElement(imageIcon, minWidth: 60, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 60);

                    //NAME ITEM
                    Text itemName = UIFactory.CreateLabel(_contentProduct, "itemNameTxt-" + index, $" {item.getItemName()}", TextAnchor.MiddleLeft);
                    UIFactory.SetLayoutElement(itemName.gameObject, minWidth: 310, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 310);

                    // PRICE ITEM
                    Text itemPrice = UIFactory.CreateLabel(_contentProduct, "itemPriceTxt-" + index, $"{item.price} {coin.Name}");
                    UIFactory.SetLayoutElement(itemPrice.gameObject, minWidth: 100, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 100);

                    // DELETE BTN
                    ButtonRef deleteBtn = UIFactory.CreateButton(_contentProduct, "deleteItemBtn-" + index, "Delete", new Color(0.3f, 0.2f, 0.2f));
                    UIFactory.SetLayoutElement(deleteBtn.Component.gameObject, minWidth: 100, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 100);
                    deleteBtn.ButtonText.color = new Color(44 / 255f, 62 / 255f, 80 / 255f);
                    RuntimeHelper.SetColorBlock(deleteBtn.Component,
                       new Color(212 / 255f, 172 / 255f, 13 / 255f),
                        new Color(231 / 255f, 76 / 255f, 60 / 255f),
                        new Color(236 / 255f, 112 / 255f, 99 / 255f));
                    deleteBtn.OnClick += DeleteAction;

                    UIFactory.SetLayoutElement(_contentProduct, flexibleHeight: 0, minHeight: 60, preferredHeight: 60, flexibleWidth: 0);
                    index--;
                    productsListLayers.Add(_contentProduct);

                    // FAKE LINE
                    var _separator = UIFactory.CreateHorizontalGroup(contentScroll, "Separator-" + index, true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));
                    var fakeTXT = UIFactory.CreateLabel(_separator, "FakeTextt-" + index, "", TextAnchor.MiddleCenter);
                    UIFactory.SetLayoutElement(fakeTXT.gameObject, minWidth: MinWidth, minHeight: 2, flexibleHeight: 0, preferredHeight: 2, flexibleWidth: 9999, preferredWidth: MinWidth);
                    productsListLayers.Add(_separator);
                }
            }
        }

        private void TypeDropdownValueChanged(int value)
        {
            if(value != 0)
            {
                var nameType = itemsType[value];
                itemsGame = ClientDB.getAllItemsByType(nameType);
                dropdownitem.Hide();
                dropdownitem.ClearOptions();
                dropdownitem.options.Clear();
                foreach (var item in itemsGame)
                {
                    dropdownitem.options.Add(new Dropdown.OptionData(item));
                }
                dropdownitem.value = 0;
            } 
        }

        private void ItemDropdownValueChanged(int value)
        {
            dropdowntype.Hide();
            if (value != 0)
            {
                var nameArray = itemsGame[value].Split("| ");
                itemNewAdd = Int32.Parse(nameArray[1]);
            }
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
            var btnName = EventSystem.current.currentSelectedGameObject.name;
            var indexToDelete = btnName.Replace("deleteItemBtn-", "");
            var msg = new DeleteSerializedMessage()
            {
                Item = indexToDelete
            };
            ClientDeleteMessageAction.Send(msg);
            RefreshAction();

            Plugin.Logger.LogInfo(indexToDelete);
        }
        private void RefreshAction()
        {
            UIManager.RefreshDataPanel();
        }
        public void RefreshData()
        {
            //UIManager.RefreshDataPanel();
            foreach (var product in productsListLayers)
            {
                UnityEngine.Object.Destroy(product, 0.2f);
            }
            productsListLayers = new List<GameObject>();
            var _contentProduct = UIFactory.CreateHorizontalGroup(contentScroll, "ContentItem", true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

            // Alert ITEM
            alertTXT = new Text();
            alertTXT = UIFactory.CreateLabel(_contentProduct, "AlertTxt", $"Refreshing...", TextAnchor.MiddleCenter);
            UIFactory.SetLayoutElement(alertTXT.gameObject, flexibleHeight: 9999, flexibleWidth: 0);
            
        }


        private void OpenCloseAction()
        {
            if (ClientDB.shopOpen)
            {
                ClientDB.shopOpen = false;
                OpenCloseStoreBtn.Component.GetComponentInChildren<Text>().text = "Open Store";
                RuntimeHelper.SetColorBlock(OpenCloseStoreBtn.Component,
                      new Color(17 / 255f, 122 / 255f, 101 / 255f),
                       new Color(19 / 255f, 141 / 255f, 117 / 255f),
                       new Color(22 / 255f, 160 / 255f, 133 / 255f));

                ClientCloseMessageAction.Send();
            } else
            {
                ClientDB.shopOpen = true;
                OpenCloseStoreBtn.Component.GetComponentInChildren<Text>().text = "Close Store";
                RuntimeHelper.SetColorBlock(OpenCloseStoreBtn.Component,
                       new Color(33 / 255f, 47 / 255f, 60 / 255f),
                        new Color(40 / 255f, 55 / 255f, 71 / 255f),
                        new Color(93 / 255f, 109 / 255f, 126 / 255f));
                
                ClientOpenMessageAction.Send();
            }
            
        }

    }
}
