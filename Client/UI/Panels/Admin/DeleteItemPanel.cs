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

namespace BloodyShop.Client.UI.Panels.Admin
{
    public class DeleteItemPanel : UniverseLib.UI.Panels.PanelBase
    {

        public static DeleteItemPanel Instance { get; private set; }

        public override string Name => "Admin Shop";

        public override bool CanDragAndResize => true;

        public override int MinWidth => 680;
        public override int MinHeight => 535;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 1f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 1f);
        public override Vector2 DefaultPosition => new Vector2(0 - MinWidth / 2 + 680, 0 + MinWidth / 2);

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

        public static bool active = false;

        private static string[] itemsGame { get; set; }
        private static string[] itemsType { get; set; }

        public DeleteItemPanel(UIBase owner) : base(owner)
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



            //INSERT LAYOUT
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

            // CONTAINER FOR PRODUCTS
            var _contentHeader = UIFactory.CreateHorizontalGroup(ContentRoot.gameObject, "HeaderItem", true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

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

            // DELETE BTN
            var headerDelete = UIFactory.CreateUIObject("BuyItem", _contentHeader);
            UIFactory.SetLayoutElement(headerDelete, minWidth: 110, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 110);

            UIFactory.SetLayoutElement(_contentHeader, flexibleHeight: 0, minHeight: 60, preferredHeight: 60, flexibleWidth: 0);

            contentScroll = new GameObject();

            var _scroolView = UIFactory.CreateScrollView(ContentRoot.gameObject, "scrollView", out contentScroll, out AutoSliderScrollbar autoSliderScrollbar);

            CreateListProductsLayou();

            SetActive(true);

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

    }
}
