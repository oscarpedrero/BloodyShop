using BloodyShop.DB;
using VRising.GameData.Models;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UnityEngine.EventSystems;
using BloodyShop.Network.Messages;
using BloodyShop.Client.Network;
using UniverseLib.UI.Widgets;
using UniverseLib;
using System.Collections.Generic;
using BloodyShop.DB.Models;
using System.Linq;

namespace BloodyShop.Client.UI.Panels.Admin
{
    public class DeleteItemPanel : UniverseLib.UI.Panels.PanelBase
    {

        public static DeleteItemPanel Instance { get; private set; }

        public override string Name => "Admin Delete Product";

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
        public GameObject contentScroll;
        public Dropdown dropdowntype;
        public Dropdown dropdownitem;
        public ButtonRef OpenCloseStoreBtn;
        public List<GameObject> productsListLayers = new List<GameObject>();
        public InputFieldRef searchInputTXT; 
        public GameObject ContentPagination;

        public List<PrefabModel> items = new();

        public static List<(int index, int input)> _stackArrayCache = new();

        public static float CurrentPanelWidth => Instance.Rect.rect.width;
        public static float CurrentPanelHeight => Instance.Rect.rect.height;

        public int CurrentDisplayedIndex;

        public static bool active = false;

        private static int limit = 10;
        private static int page = 0;
        private static int total = 0;
        private static int skip = 0;
        public static string textSearch = "";

        public DeleteItemPanel(UIBase owner) : base(owner)
        {
            Instance = this;
        }

        public override void Update()
        {
            
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

            CreateListProductsLayout();

            SetActive(true);

        }

        public void CreateListProductsLayout()
        {

            items = ItemsDB.searchItemByNameForShop(textSearch);
            total = items.Count;

            productsListLayers = new List<GameObject>();
            skip = page * limit;
            if (total > 0 && skip >= total)
            {
                page--;
                if (page < 0)
                {
                    page = 0;
                }
                skip = page * limit;
            }
            var index = skip + 1;
            
            foreach (var item in items.Skip(skip).Take(10))
            {
                if (ShareDB.getCoin(out PrefabModel coin))
                {
                    // CONTAINER FOR PRODUCTS
                    var _contentProduct = UIFactory.CreateHorizontalGroup(contentScroll, "ContentItem-" + index, true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

                    // Aval ITEM
                    Text itemAval = UIFactory.CreateLabel(_contentProduct, "itemAvalTxt-" + index, $"{item.PrefabStock}", TextAnchor.MiddleCenter);
                    UIFactory.SetLayoutElement(itemAval.gameObject, minWidth: 50, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 50);

                    // ITEM ICON
                    var imageIcon = UIFactory.CreateUIObject("IconItem-" + index, _contentProduct);
                    var iconImage = imageIcon.AddComponent<Image>();
                    iconImage.sprite = item.PrefabIcon;
                    UIFactory.SetLayoutElement(imageIcon, minWidth: 60, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 60);

                    //NAME ITEM
                    Text itemName = UIFactory.CreateLabel(_contentProduct, "itemNameTxt-" + index, $" {item.PrefabStack}x {item.PrefabName}", TextAnchor.MiddleLeft);
                    UIFactory.SetLayoutElement(itemName.gameObject, minWidth: 310, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 310);

                    // PRICE ITEM
                    Text itemPrice = UIFactory.CreateLabel(_contentProduct, "itemPriceTxt-" + index, $"{item.PrefabPrice} {coin.PrefabName}");
                    UIFactory.SetLayoutElement(itemPrice.gameObject, minWidth: 100, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 100);
                    _stackArrayCache.Add((index, item.PrefabStack));

                    // DELETE BTN
                    ButtonRef deleteBtn = UIFactory.CreateButton(_contentProduct, "deleteItemBtn-" + index, "Delete", new Color(203 / 255f, 67 / 255f, 53 / 255f));
                    UIFactory.SetLayoutElement(deleteBtn.Component.gameObject, minWidth: 100, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 100);
                    deleteBtn.OnClick += DeleteAction;

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

            createPagination();

        }

        private void createPagination()
        {

            decimal totalPages = total / limit;
            var last = Math.Ceiling(totalPages);

            //INSERT LAYOUT
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

            ContentPagination = new GameObject();

            // CONTAINER FOR PAGINATION
            ContentPagination = UIFactory.CreateHorizontalGroup(ContentRoot.gameObject, "PaginationGroup", true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

            Text footerText = UIFactory.CreateLabel(ContentPagination, "footerText", $"Products: {total} Pages: {last + 1 }");
            UIFactory.SetLayoutElement(footerText.gameObject, minWidth: 50, minHeight: 30, flexibleHeight: 0, preferredHeight: 30, flexibleWidth: 0, preferredWidth: 50);
            if (items.Count > limit)
            {
                var pageNumber = page + 1;
                if ((page - 1) >= 0)
                {
                    // Previous BTN
                    ButtonRef previousBtn = UIFactory.CreateButton(ContentPagination, "previousBtn", $"<- Page {pageNumber - 1}");
                    RuntimeHelper.SetColorBlockAuto(previousBtn.Component,
                       new Color(23 / 255f, 165 / 255f, 137 / 255f)
                       );
                    UIFactory.SetLayoutElement(previousBtn.Component.gameObject, minWidth: 150, minHeight: 30, flexibleHeight: 0, preferredHeight: 30, flexibleWidth: 0, preferredWidth: 150);
                    previousBtn.OnClick += changePage;
                }
                
                if ((page + 1)<=last)
                {
                    
                    // Next BTN
                    ButtonRef nextBtn = UIFactory.CreateButton(ContentPagination, "nextBtn", $"Page {pageNumber + 1} ->");
                    RuntimeHelper.SetColorBlockAuto(nextBtn.Component,
                        new Color(23 / 255f, 165 / 255f, 137 / 255f)
                        );
                    UIFactory.SetLayoutElement(nextBtn.Component.gameObject, minWidth: 150, minHeight: 30, flexibleHeight: 0, preferredHeight: 30, flexibleWidth: 0, preferredWidth: 150);
                    nextBtn.OnClick += changePage;
                }

            }

        }

        private void SearchActionText(string str)
        {
            textSearch = str.ToLower();
            page = 0;
            RefreshData();
            CreateListProductsLayout();

        }

        private void SearchAction()
        {
            textSearch = searchInputTXT.Text.ToLower();
            page = 0;
            RefreshData();
            CreateListProductsLayout();
        }


        private void DeleteAction()
        {
            var btnName = EventSystem.current.currentSelectedGameObject.name;
            var indexItemUI = btnName.Replace("deleteItemBtn-", "");
            var prefabDelete = items[Int32.Parse(indexItemUI) - 1];
            var stackDel = serachStackInput(Int32.Parse(indexItemUI));
            indexItemUI = ItemsDB.searchIndexForProduct(prefabDelete.PrefabGUID, stackDel).ToString();

            //Plugin.Logger.LogInfo($"DELETE INDEX: {indexItemUI}");

            if (indexItemUI != "-1")
            {
                var msg = new DeleteSerializedMessage()
                {
                    Item = indexItemUI
                };
                ClientDeleteMessageAction.Send(msg);
                RefreshAction();
            }
        }

        private int serachStackInput(int indexSearch)
        {

            foreach (var (index, input) in _stackArrayCache)
            {
                if (index == indexSearch)
                {

                    if (input == 0)
                    {
                        return 1;
                    }

                    return input;
                }
            }

            return 1;
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
                UnityEngine.Object.Destroy(product, 0f);
            }
            productsListLayers = new List<GameObject>();
            var _contentProduct = UIFactory.CreateHorizontalGroup(contentScroll, "ContentItem", true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

            UnityEngine.Object.Destroy(ContentPagination, 0f);

        }

        private void changePage()
        {
            var btnName = EventSystem.current.currentSelectedGameObject.name;
            if (btnName == "nextBtn")
            {
                    page++;
            }
            else
            {
                    page--;
            }

            RefreshData();
            CreateListProductsLayout();


        }

    }
}
