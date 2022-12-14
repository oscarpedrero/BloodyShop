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
using System.Collections.Generic;
using System.Linq;
using BloodyShop.DB.Models;

namespace BloodyShop.Client.UI.Panels.Admin
{
    public class AddItemPanel : UniverseLib.UI.Panels.PanelBase
    {

        public static AddItemPanel Instance { get; private set; }

        public override string Name => "Admin Add Item Shop";

        public override bool CanDragAndResize => true;

        public override int MinWidth => 680;
        public override int MinHeight => 535;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 1f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 1f);
        public override Vector2 DefaultPosition => new Vector2(0 - MinWidth / 2 - 680, 0 + MinWidth / 2);

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

        public List<PrefabModel> itemsModel = new();

        public static float CurrentPanelWidth => Instance.Rect.rect.width;
        public static float CurrentPanelHeight => Instance.Rect.rect.height;

        public int CurrentDisplayedIndex;

        public static bool active = false;

        InputFieldRef searchInputTXT;

        List<InputFieldRef> stockArray = new();

        List<InputFieldRef> priceArray = new();

        private static int limit = 10;

        public AddItemPanel(UIBase owner) : base(owner)
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
            GameObject closeHolder = TitleBar.transform.Find("CloseHolder").gameObject;

            //INSERT LAYOUT
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

            // CONTAINER FOR SEARCH INPUT
            var _contentSearch = UIFactory.CreateHorizontalGroup(ContentRoot.gameObject, "HeaderItem", true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

            UIFactory.SetLayoutElement(_contentSearch, flexibleHeight: 0, minHeight: 60, preferredHeight: 60, flexibleWidth: 0);

            // SEARCH INPUT ITEM
            searchInputTXT = UIFactory.CreateInputField(_contentSearch, "SearchInput", "Search Text");
            UIFactory.SetLayoutElement(searchInputTXT.GameObject, minWidth: MinWidth - 100, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 9999, preferredWidth: MinWidth - 100);
            searchInputTXT.OnValueChanged += SearchActionText;

            // SEARCH BTN
            ButtonRef searchBtn = UIFactory.CreateButton(_contentSearch, "saveBtn-", "Search", new Color(52 / 255f, 73 / 255f, 94 / 255f));
            UIFactory.SetLayoutElement(searchBtn.Component.gameObject, minWidth: 80, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 80);
            searchBtn.OnClick += SearchAction;


            //INSERT LAYOUT
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

            // CONTAINER FOR PRODUCTS
            var _contentHeader = UIFactory.CreateHorizontalGroup(ContentRoot.gameObject, "HeaderItem", true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

            // ITEM ICON
            Text headerName = UIFactory.CreateLabel(_contentHeader, "itemNameTxt", $"Name", TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(headerName.gameObject, minWidth: 60, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 60);

            //NAME ITEM
            var imageHeader = UIFactory.CreateUIObject("IconItem", _contentHeader);
            UIFactory.SetLayoutElement(imageHeader, minWidth: 310, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 310);

            // PRICE ITEM
            Text headerPrice = UIFactory.CreateLabel(_contentHeader, "itemPriceTxt", $"Price");
            UIFactory.SetLayoutElement(headerPrice.gameObject, minWidth: 100, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 100);

            // STOCK ITEM
            Text headerAval = UIFactory.CreateLabel(_contentHeader, "itemAvalTxt", $"Stock");
            UIFactory.SetLayoutElement(headerAval.gameObject, minWidth: 50, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 50);

            // ADDITEM BTN
            var headerAdd = UIFactory.CreateUIObject("AddItem", _contentHeader);
            UIFactory.SetLayoutElement(headerAdd, minWidth: 110, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 110);

            UIFactory.SetLayoutElement(_contentHeader, flexibleHeight: 0, minHeight: 60, preferredHeight: 60, flexibleWidth: 0);

            contentScroll = new GameObject();

            var _scroolView = UIFactory.CreateScrollView(ContentRoot.gameObject, "scrollView", out contentScroll, out AutoSliderScrollbar autoSliderScrollbar);

            CreateListProductsLayout();

            SetActive(true);

        }

        public void CreateListProductsLayout()
        {

            var index = 0;
            productsListLayers = new List<GameObject>();
            stockArray = new();
            priceArray = new();
            foreach (var item in itemsModel.Take(limit))
            {
                if (ShareDB.getCoin(out ItemModel coin))
                {
                    // CONTAINER FOR PRODUCTS
                    var _contentProduct = UIFactory.CreateHorizontalGroup(contentScroll, "ContentItem-" + index, true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

                    // ITEM ICON
                    var imageIcon = UIFactory.CreateUIObject("IconItem-" + index, _contentProduct);
                    var iconImage = imageIcon.AddComponent<Image>();
                    iconImage.sprite = item?.PrefabIcon;
                    UIFactory.SetLayoutElement(imageIcon, minWidth: 60, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 60);

                    //NAME ITEM
                    Text itemName = UIFactory.CreateLabel(_contentProduct, "itemNameTxt" + index, $" {item.PrefabName}", TextAnchor.MiddleLeft);
                    UIFactory.SetLayoutElement(itemName.gameObject, minWidth: 310, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 310);

                    // PRICE ITEM
                    var priceNew = UIFactory.CreateInputField(_contentProduct, "priceNew|" + index, "Price");
                    UIFactory.SetLayoutElement(priceNew.GameObject, minWidth: 80, minHeight: 30, flexibleHeight: 0, preferredHeight: 30, flexibleWidth: 0, preferredWidth: 100);
                    priceNew.Component.contentType = InputField.ContentType.IntegerNumber;
                    priceNew.Text.PadLeft(2);

                    priceArray.Add(priceNew);

                    // STOCK ITEM
                    var stockNew = UIFactory.CreateInputField(_contentProduct, "stockNew|" + index, "Stock");
                    UIFactory.SetLayoutElement(stockNew.GameObject, minWidth: 70, minHeight: 30, flexibleHeight: 0, preferredHeight: 30, flexibleWidth: 0, preferredWidth: 50);
                    stockNew.Component.contentType = InputField.ContentType.IntegerNumber;
                    stockNew.Text.PadLeft(2);

                    stockArray.Add(stockNew);

                    // SAVE BTN
                    ButtonRef saveBtn = UIFactory.CreateButton(_contentProduct, "saveBtn|" + index, "Add Item", new Color(183 / 255f, 149 / 255f, 11 / 255f));
                    UIFactory.SetLayoutElement(saveBtn.Component.gameObject, minWidth: 100, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 100);
                    saveBtn.OnClick += SaveAction;

                    UIFactory.SetLayoutElement(_contentProduct, flexibleHeight: 0, minHeight: 60, preferredHeight: 60, flexibleWidth: 0);
                    productsListLayers.Add(_contentProduct);

                    // FAKE LINE
                    var _separator = UIFactory.CreateHorizontalGroup(contentScroll, "Separator-" + index, true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));
                    var fakeTXT = UIFactory.CreateLabel(_separator, "FakeTextt-" + index, "", TextAnchor.MiddleCenter);
                    UIFactory.SetLayoutElement(fakeTXT.gameObject, minWidth: MinWidth, minHeight: 2, flexibleHeight: 0, preferredHeight: 2, flexibleWidth: 9999, preferredWidth: MinWidth);
                    productsListLayers.Add(_separator);

                    index++;
                }
            }
        }

        private void SearchActionText(string str)
        {
            
            itemsModel = ItemsDB.searchItemByNameForAdd(str.ToLower());
            RefreshData();

        }

        private void SearchAction()
        {
            
            itemsModel = ItemsDB.searchItemByNameForAdd(searchInputTXT.Text.ToLower());
            RefreshData();
        }

        private void SaveAction()
        {
            var btnName = EventSystem.current.currentSelectedGameObject.name;
            var index = Int32.Parse(btnName.Replace("saveBtn|", ""));
            var item = itemsModel[index].PrefabGUID;
            
            var inputPrice = priceArray[index];
            var inputStock = stockArray[index];

            var price = inputPrice.Text;
            var stock = inputStock.Text;
            if (price != "" || stock != "")
            {
                var msg = new AddSerializedMessage()
                {
                    PrefabGUID = item.ToString(),
                    Price = price,
                    Stock = stock,
                };
                ClientAddMessageAction.Send(msg);
                RefreshAction();
            }
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

            CreateListProductsLayout();

        }

    }
}
