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
using MS.Internal.Xml.XPath;
using BloodyShop.Client.Utils;

namespace BloodyShop.Client.UI.Panels.Admin
{
    public class AddItemPanel : UIModel
    {
        public PanelConfig Parent { get; }

        public static AddItemPanel Instance { get; private set; }

        public GameObject NavbarHolder;
        public GameObject ContentHolder;
        public GameObject ContentProduct;
        public RectTransform ContentRect;
        public Text alertTXT;
        public GameObject contentScroll;
        public Dropdown dropdownCurrency;
        public Dropdown dropdownitem;
        public ButtonRef OpenCloseStoreBtn;
        public List<GameObject> productsListLayers = new List<GameObject>();

        public List<PrefabModel> itemsModel = new();

        public List<CurrencyModel> currencies { get; private set; }

        public int CurrentDisplayedIndex;

        public static bool active = false;

        InputFieldRef searchInputTXT;

        List<InputFieldRef> stockArray = new();

        List<InputFieldRef> stackArray = new();

        List<InputFieldRef> priceArray = new();

        List<Dropdown> currencyArray = new();

        private static int limit = 10;

        private static GameObject uiRoot;

        public static int MinWidth => 680;

        public override GameObject UIRoot => uiRoot;

        public AddItemPanel(PanelConfig parent)
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
            Text headerName = UIFactory.CreateLabel(_contentHeader, "itemNameTxt", $"Name", TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(headerName.gameObject, minWidth: 60, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 60);

            //NAME ITEM
            var imageHeader = UIFactory.CreateUIObject("IconItem", _contentHeader);
            UIFactory.SetLayoutElement(imageHeader, minWidth: 310, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 310);

            // CURRENCY ITEM
            Text headerCurrency = UIFactory.CreateLabel(_contentHeader, "itemCurrencyTxt", $"Currency");
            UIFactory.SetLayoutElement(headerCurrency.gameObject, minWidth: 50, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 50);

            // PRICE ITEM
            Text headerPrice = UIFactory.CreateLabel(_contentHeader, "itemPriceTxt", $"Price");
            UIFactory.SetLayoutElement(headerPrice.gameObject, minWidth: 100, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 100);

            // STOCK ITEM
            Text headerAval = UIFactory.CreateLabel(_contentHeader, "itemAvalTxt", $"Stock");
            UIFactory.SetLayoutElement(headerAval.gameObject, minWidth: 50, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 50);

            // STACK ITEM
            Text headerStack = UIFactory.CreateLabel(_contentHeader, "itemStackTxt", $"Stack");
            UIFactory.SetLayoutElement(headerAval.gameObject, minWidth: 50, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 50);

            // ADDITEM BTN
            var headerAdd = UIFactory.CreateUIObject("AddItem", _contentHeader);
            UIFactory.SetLayoutElement(headerAdd, minWidth: 110, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 110);

            UIFactory.SetLayoutElement(_contentHeader, flexibleHeight: 0, minHeight: 60, preferredHeight: 60, flexibleWidth: 0);

            contentScroll = new GameObject();

            var _scroolView = UIFactory.CreateScrollView(uiRoot.gameObject, "scrollView", out contentScroll, out AutoSliderScrollbar autoSliderScrollbar);

            CreateListProductsLayout();

            SetActive(true);

        }

        public void CreateListProductsLayout()
        {

            var index = 0;
            productsListLayers = new List<GameObject>();
            stockArray = new();
            stackArray = new();
            priceArray = new();
            currencyArray = new();
            foreach (var item in itemsModel.Take(limit))
            {
                    currencies = ShareDB.getCurrencyList();


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

                    string[] dropDownCurrencyValue = currencies.Select(currency => currency.name).ToArray();

                    // CURRENCY ITEM
                    GameObject currencyNew = UIFactory.CreateDropdown(_contentProduct, "currencyNew|" + index, out dropdownCurrency, "Currency", 14, OnDropdownSelect, dropDownCurrencyValue);
                    UIFactory.SetLayoutElement(currencyNew, minWidth: 80, minHeight: 30, flexibleHeight: 0, preferredHeight: 30, flexibleWidth: 0, preferredWidth: 100);

                    currencyArray.Add(dropdownCurrency);

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

                    // STACK ITEM
                    var stackNew = UIFactory.CreateInputField(_contentProduct, "stackNew|" + index, "Stack");
                    UIFactory.SetLayoutElement(stackNew.GameObject, minWidth: 70, minHeight: 30, flexibleHeight: 0, preferredHeight: 30, flexibleWidth: 0, preferredWidth: 50);
                    stackNew.Component.contentType = InputField.ContentType.IntegerNumber;
                    stackNew.Text = "1";
                    stackNew.Text.PadLeft(2);

                    stackArray.Add(stackNew);

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

        public static void OnDropdownSelect(int index)
        {

            return;

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
            var name = itemsModel[index].PrefabName;
            
            var inputPrice = priceArray[index];
            var inputStock = stockArray[index];
            var inputStack = stackArray[index];

            var dropDownCurrency = currencyArray[index];

            var currency = ShareDB.getCurrencyByName(dropDownCurrency.options[dropDownCurrency.value].text);

            var price = inputPrice.Text;
            var stock = inputStock.Text;
            var stack = inputStack.Text;
            if (price != "" || stock != "")
            {
                if(stack == "")
                {
                    stack = "1";
                }

                var msg = new AddSerializedMessage()
                {
                    PrefabGUID = item.ToString(),
                    Name = name,
                    Price = price,
                    Stock = stock,
                    Stack = stack,
                    CurrencyGUID = currency.guid.ToString(),
                };
                Sound.Play(Properties.Resources.coin);
                ClientAddMessageAction.Send(msg);
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
            foreach (var product in productsListLayers)
            {
                UnityEngine.Object.Destroy(product, 0.2f);
            }
            productsListLayers = new List<GameObject>();

            CreateListProductsLayout();

        }

    }
}
