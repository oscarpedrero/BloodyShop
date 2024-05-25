using BloodyShop.DB;
using Bloody.Core.Models.v1;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using BloodyShop.Client.DB;
using UnityEngine.EventSystems;
//using BloodyShop.Network.Messages;
//using BloodyShop.Client.Network;
using UniverseLib.UI.Widgets;
using System.Collections.Generic;
using System.Linq;
using BloodyShop.DB.Models;
using MS.Internal.Xml.XPath;
using BloodyShop.Client.Utils;

namespace BloodyShop.Client.UI.Panels.Admin
{
    public class AddCurrencyPanel : UIModel
    {
        public PanelConfig Parent { get; }

        public static AddCurrencyPanel Instance { get; private set; }

        public GameObject NavbarHolder;
        public GameObject ContentHolder;
        public GameObject ContentProduct;
        public RectTransform ContentRect;
        public Text alertTXT;
        public GameObject contentScroll;
        public Dropdown dropdownCurrency;
        public Dropdown dropdownitem;
        public ButtonRef OpenCloseStoreBtn;
        public List<GameObject> currencyListLayers = new List<GameObject>();

        public List<PrefabModel> currencyModel = new();

        public int CurrentDisplayedIndex;

        public static bool active = false;

        InputFieldRef searchInputTXT;

        private static int limit = 10;

        private static GameObject uiRoot;

        public static int MinWidth => 680;

        public override GameObject UIRoot => uiRoot;

        List<Dropdown> currencyDropArray = new();

        public AddCurrencyPanel(PanelConfig parent)
        {
            Parent = parent;
        }

        public override void ConstructUI(GameObject content)
        {
            active = true;

            uiRoot = UIFactory.CreateUIObject("ConfigPanel", content);

            // TITLE Bar
            //GameObject closeHolder = TitleBar.transform.Find("CloseHolder").gameObject;

            //INSERT LAYOUT
            //UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

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
            var imageHeader = UIFactory.CreateUIObject("IconItem", _contentHeader);
            UIFactory.SetLayoutElement(imageHeader, minWidth: 60, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 60);

            //NAME ITEM
            Text headerName = UIFactory.CreateLabel(_contentHeader, "itemNameTxt", $"Name", TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(headerName.gameObject, minWidth: 310, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 310);

            //DROP ITEM
            Text headerDrop = UIFactory.CreateLabel(_contentHeader, "itemDropTxt", $"Drop", TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(headerName.gameObject, minWidth: 60, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 60);

            // ADD Currency BTN
            var headerAdd = UIFactory.CreateUIObject("AddCurrency", _contentHeader);
            UIFactory.SetLayoutElement(headerAdd, minWidth: 100, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 100);

            UIFactory.SetLayoutElement(_contentHeader, flexibleHeight: 0, minHeight: 60, preferredHeight: 60, flexibleWidth: 0);

            contentScroll = new GameObject();

            var _scroolView = UIFactory.CreateScrollView(uiRoot.gameObject, "scrollView", out contentScroll, out AutoSliderScrollbar autoSliderScrollbar);

            CreateListCurrenciesLayout();

            SetActive(true);

        }

        public void CreateListCurrenciesLayout()
        {

            var index = 0;
            currencyListLayers = new List<GameObject>();
            currencyDropArray = new List<Dropdown>();
            foreach (var item in currencyModel.Take(limit))
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

                string[] dropDownValue = {"True","False"};

                // DROP ITEM
                GameObject currencyDropNew = UIFactory.CreateDropdown(_contentProduct, "currencyNew|" + index, out dropdownCurrency, "Currency", 14, OnDropdownSelect, dropDownValue);
                UIFactory.SetLayoutElement(currencyDropNew, minWidth: 60, minHeight: 30, flexibleHeight: 0, preferredHeight: 30, flexibleWidth: 0, preferredWidth: 60);

                currencyDropArray.Add(dropdownCurrency);

                // SAVE BTN
                ButtonRef saveBtn = UIFactory.CreateButton(_contentProduct, "saveBtn|" + index, "Add Currency", new Color(183 / 255f, 149 / 255f, 11 / 255f));
                UIFactory.SetLayoutElement(saveBtn.Component.gameObject, minWidth: 200, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 200);
                saveBtn.OnClick += SaveAction;

                UIFactory.SetLayoutElement(_contentProduct, flexibleHeight: 0, minHeight: 60, preferredHeight: 60, flexibleWidth: 0);
                currencyListLayers.Add(_contentProduct);

                // FAKE LINE
                var _separator = UIFactory.CreateHorizontalGroup(contentScroll, "Separator-" + index, true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));
                var fakeTXT = UIFactory.CreateLabel(_separator, "FakeTextt-" + index, "", TextAnchor.MiddleCenter);
                UIFactory.SetLayoutElement(fakeTXT.gameObject, minWidth: MinWidth, minHeight: 2, flexibleHeight: 0, preferredHeight: 2, flexibleWidth: 9999, preferredWidth: MinWidth);
                currencyListLayers.Add(_separator);

                index++;
                
            }
        }

        public static void OnDropdownSelect(int index)
        {

            return;

        }

        private void SearchActionText(string str)
        {
            
            currencyModel = ItemsDB.searchItemByNameForAdd(str.ToLower());
            RefreshData();

        }

        private void SearchAction()
        {

            currencyModel = ItemsDB.searchItemByNameForAdd(searchInputTXT.Text.ToLower());
            RefreshData();
        }

        private void SaveAction()
        {
            var btnName = EventSystem.current.currentSelectedGameObject.name;
            var index = Int32.Parse(btnName.Replace("saveBtn|", ""));
            var item = currencyModel[index].PrefabGUID;
            var name = currencyModel[index].PrefabName;
            var dropDownCurrency = currencyDropArray[index];
            var drop = dropDownCurrency.options[dropDownCurrency.value].text;
            Plugin.Logger.LogWarning(drop);

            /*var msg = new AddCurrencySerializedMessage()
            {
                CurrencyGUID = item.ToString(),
                Name = name,
                Drop = drop.ToString(),
            };
            Sound.Play(Properties.Resources.coin);
            ClientAddCurrencyMessageAction.Send(msg);*/
            RefreshAction();

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
                UnityEngine.Object.Destroy(product, 0.2f);
            }
            currencyListLayers = new List<GameObject>();

            CreateListCurrenciesLayout();

        }

    }
}
