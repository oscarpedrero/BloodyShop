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
using BloodyShop.Client.Utils;
using VRising.GameData;
using ProjectM;

namespace BloodyShop.Client.UI.Panels.Admin
{
    public class DeleteCurrencyPanel : UIModel
    {

        public static DeleteCurrencyPanel Instance { get; private set; }

        

        public static int MinWidth => 680;

        public GameObject NavbarHolder;
        public Dropdown MouseInspectDropdown;
        public GameObject ContentHolder;
        public GameObject ContentProduct;
        public RectTransform ContentRect;
        public GameObject contentScroll;
        public List<GameObject> currencyListLayers = new List<GameObject>();
        public InputFieldRef searchInputTXT; 
        public GameObject ContentPagination;

        public List<CurrencyModel> currencies = new();

        public int CurrentDisplayedIndex;

        public static bool active = false;

        private static int limit = 10;
        private static int page = 0;
        private static int total = 0;
        private static int skip = 0;
        public static string textSearch = "";

        public PanelConfig Parent { get; }

        public DeleteCurrencyPanel(PanelConfig parent)
        {
            Parent = parent;
        }

        private static GameObject uiRoot;

        public override GameObject UIRoot => uiRoot;

        public override void ConstructUI(GameObject content)
        {

            active = true;

            uiRoot = UIFactory.CreateUIObject("SceneExplorer", content);

            // CONTAINER FOR SEARCH INPUT
            var _contentSearch = UIFactory.CreateHorizontalGroup(uiRoot, "HeaderItem", true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

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
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(uiRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

            // CONTAINER FOR PRODUCTS
            var _contentHeader = UIFactory.CreateHorizontalGroup(uiRoot, "HeaderItem", true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

            // ITEM ICON
            Text headerName = UIFactory.CreateLabel(_contentHeader, "itemNameTxt", $"Name", TextAnchor.MiddleLeft);

            UIFactory.SetLayoutElement(headerName.gameObject, minWidth: 60, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 60);

            //NAME ITEM
            var imageHeader = UIFactory.CreateUIObject("IconItem", _contentHeader);
            UIFactory.SetLayoutElement(imageHeader, minWidth: 350, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 310);

            // DELETE BTN
            var headerDelete = UIFactory.CreateUIObject("BuyItem", _contentHeader);
            UIFactory.SetLayoutElement(headerDelete, minWidth: 110, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 110);

            UIFactory.SetLayoutElement(_contentHeader, flexibleHeight: 0, minHeight: 60, preferredHeight: 60, flexibleWidth: 0);

            contentScroll = new GameObject();

            var _scroolView = UIFactory.CreateScrollView(uiRoot, "scrollView", out contentScroll, out AutoSliderScrollbar autoSliderScrollbar);

            CreateListCurrenciesLayout();

            SetActive(true);

        }

        public void CreateListCurrenciesLayout()
        {

            currencies = ShareDB.searchCurrencyByNameForShop(textSearch);
            total = currencies.Count;
            currencyListLayers = new List<GameObject>();
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
            foreach (var item in currencies.Skip(skip).Take(10))
            {
                var itemModel = GameData.Items.GetPrefabById(new PrefabGUID(item.guid));
                // CONTAINER FOR PRODUCTS
                var _contentProduct = UIFactory.CreateHorizontalGroup(contentScroll, "ContentItem-" + index, true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));
                // ITEM ICON
                var imageIcon = UIFactory.CreateUIObject("IconItem-" + index, _contentProduct);
                var iconImage = imageIcon.AddComponent<Image>();
                iconImage.sprite = itemModel.ManagedGameData.ManagedItemData?.Icon;
                UIFactory.SetLayoutElement(imageIcon, minWidth: 60, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 60);

                //NAME ITEM
                Text itemName = UIFactory.CreateLabel(_contentProduct, "itemNameTxt-" + index, $" {item.name}", TextAnchor.MiddleLeft);
                UIFactory.SetLayoutElement(itemName.gameObject, minWidth: 550, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 550);

                // DELETE BTN
                ButtonRef deleteBtn = UIFactory.CreateButton(_contentProduct, "deleteItemBtn-" + index, "Delete", new Color(203 / 255f, 67 / 255f, 53 / 255f));
                UIFactory.SetLayoutElement(deleteBtn.Component.gameObject, minWidth: 150, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 150);
                deleteBtn.OnClick += DeleteAction;

                UIFactory.SetLayoutElement(_contentProduct, flexibleHeight: 0, minHeight: 60, preferredHeight: 60, flexibleWidth: 0);
                    
                currencyListLayers.Add(_contentProduct);

                // FAKE LINE
                var _separator = UIFactory.CreateHorizontalGroup(contentScroll, "Separator-" + index, true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));
                var fakeTXT = UIFactory.CreateLabel(_separator, "FakeTextt-" + index, "", TextAnchor.MiddleCenter);
                UIFactory.SetLayoutElement(fakeTXT.gameObject, minWidth: MinWidth, minHeight: 2, flexibleHeight: 0, preferredHeight: 2, flexibleWidth: 9999, preferredWidth: MinWidth);
                currencyListLayers.Add(_separator);
                index++;
            }

            createPagination();

        }

        private void createPagination()
        {

            decimal totalPages = total / limit;
            var last = System.Math.Ceiling(totalPages);

            //INSERT LAYOUT
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(uiRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

            ContentPagination = new GameObject();

            // CONTAINER FOR PAGINATION
            ContentPagination = UIFactory.CreateHorizontalGroup(uiRoot, "PaginationGroup", true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));

            Text footerText = UIFactory.CreateLabel(ContentPagination, "footerText", $"Products: {total} Pages: {last + 1 }");
            UIFactory.SetLayoutElement(footerText.gameObject, minWidth: 50, minHeight: 30, flexibleHeight: 0, preferredHeight: 30, flexibleWidth: 0, preferredWidth: 50);
            if (currencies.Count > limit)
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
            CreateListCurrenciesLayout();

        }

        private void SearchAction()
        {
            textSearch = searchInputTXT.Text.ToLower();
            page = 0;
            RefreshData();
            CreateListCurrenciesLayout();
        }


        private void DeleteAction()
        {
            Sound.Play(Properties.Resources.floop2_x);

            var btnName = EventSystem.current.currentSelectedGameObject.name;
            var indexItemUI = btnName.Replace("deleteItemBtn-", "");
            var prefabDelete = currencies[int.Parse(indexItemUI) - 1];
            indexItemUI = ShareDB.searchIdForCurrency(prefabDelete.guid).ToString();

            if (indexItemUI != "-1")
            {
                var msg = new DeleteCurrencySerializedMessage()
                {
                    Currency = indexItemUI
                };
                ClientDeleteCurrencyMessageAction.Send(msg);
                RefreshAction();
            }
        }

        public void RefreshAction()
        {
            UIManager.RefreshDataPanel();
        }
        public void RefreshData()
        {
            //UIManager.RefreshDataPanel();
            foreach (var product in currencyListLayers)
            {
                UnityEngine.Object.Destroy(product, 0f);
            }
            currencyListLayers = new List<GameObject>();
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
            CreateListCurrenciesLayout();


        }

    }
}
