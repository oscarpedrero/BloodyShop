using BloodyShop.Client.Network;
using BloodyShop.DB;
using BloodyShop.Network.Messages;
using VRising.GameData.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using BloodyShop.Client.DB;

namespace BloodyShop.Client.UI.Panels
{
    public class MainPanel : UniverseLib.UI.Panels.PanelBase
    {

        public static MainPanel Instance { get; private set; }


        /*
        public MainPanel(UIBase owner) : base(owner) { }
        public override string Name => "Shopìfy Admin";
        public override int MinWidth => 100;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new(0.25f, 0.25f);
        public override Vector2 DefaultAnchorMax => new(0.75f, 0.75f);
        public override bool CanDragAndResize => true;*/

        public override string Name => ClientDB.shopName;

        public override bool CanDragAndResize => false;

        public override int MinWidth => 530;
        public override int MinHeight => (ItemsDB.GetProductList().Count * 30) + 35;
        public override Vector2 DefaultAnchorMin => new(MinWidth, MinHeight);
        public override Vector2 DefaultAnchorMax => new(MinWidth, MinHeight);
        public override Vector2 DefaultPosition => new(Screen.height / 2, Screen.width / 2);

        public GameObject NavbarHolder;
        public Dropdown MouseInspectDropdown;
        public GameObject ContentHolder;
        public GameObject ContentProduct;
        public RectTransform ContentRect;
        public Text resultTxt;

        public static float CurrentPanelWidth => Instance.Rect.rect.width;
        public static float CurrentPanelHeight => Instance.Rect.rect.height;

        public int CurrentDisplayedIndex;

        public MainPanel(UIBase owner) : base(owner)
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

            // TitleBar
            GameObject closeHolder = this.TitleBar.transform.Find("CloseHolder").gameObject;

            // NAV BAR
            this.NavbarHolder = UIFactory.CreateGridGroup(this.ContentRoot, "Navbar", new Vector2(200, 22), new Vector2(4, 4),
                new Color(0.05f, 0.05f, 0.05f));
            UIFactory.SetLayoutElement(NavbarHolder, flexibleWidth: 9999, minHeight: 30, preferredHeight: 30, flexibleHeight: 9999);
            NavbarHolder.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var headLabel = UIFactory.CreateLabel(NavbarHolder, "HeadLabel", ClientDB.shopName, TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(headLabel.gameObject, minWidth: 50, minHeight: 50);

            if (ClientDB.userModel.IsAdmin)
            {
                // ADMIN BTN
                ButtonRef adminStoreBtn = UIFactory.CreateButton(NavbarHolder.gameObject, "AdminStoreBtn", "Admin",
                new Color(0.3f, 0.2f, 0.2f));
                UIFactory.SetLayoutElement(adminStoreBtn.Component.gameObject, minHeight: 25, minWidth: 50);
                //adminStoreBtn.Component.transform.SetSiblingIndex(adminStoreBtn.Component.transform.GetSiblingIndex() - 1);
                adminStoreBtn.OnClick += AdminPanelAction;
            }
            // REFRESH BTN
            ButtonRef refreshBtn = UIFactory.CreateButton(NavbarHolder.gameObject, "RefreshBtn", "Refresh",
                new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(refreshBtn.Component.gameObject, minHeight: 25, minWidth: 50);
            refreshBtn.OnClick += RefreshAction;
            //refreshBtn.Component.transform.SetSiblingIndex(refreshBtn.Component.transform.GetSiblingIndex() - 1);

            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(this.ContentRoot, true, true, true, true, 4, padLeft: 5, padRight: 5);

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

                    // BUY BTN
                    ButtonRef buyBtn = UIFactory.CreateButton(_contentProduct, "buyItemBtn-" + index, "BUY", new Color(0.3f, 0.2f, 0.2f));
                    UIFactory.SetLayoutElement(buyBtn.Component.gameObject, minHeight: 25, minWidth: 80);
                    buyBtn.OnClick += BuyAction;

                    UIFactory.SetLayoutElement(_contentProduct, flexibleHeight: 9999);
                    index++;
                }
            }

            this.SetActive(true);



        }

        private void AdminPanelAction()
        {
            UIManager.showAdminPanel();
        }

        private void RefreshAction()
        {
            UIManager.removePanel();
        }

        private void BuyAction()
        {
            var btnName = EventSystem.current.currentSelectedGameObject.name;
            var indexToBuy = btnName.Replace("buyItemBtn-", "");
            var msg = new BuySerializedMessage()
            {
                ItemIndex = indexToBuy
            };
            ClientBuyMessageAction.Send(msg);
            RefreshAction();
            //resultTxt.text = "Transaction completed successfully";

            Plugin.Logger.LogInfo(indexToBuy);
        }

    }
}
