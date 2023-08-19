using BloodyShop.Client.DB;
using BloodyShop.Client.Network;
using BloodyShop.Client.Utils;
using BloodyShop.Utils;
using MS.Internal.Xml.XPath;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace BloodyShop.Client.UI.Panels.Admin
{

    public class AdminMenuPanel : UniverseLib.UI.Panels.PanelBase
    {

        public static AdminMenuPanel Instance { get; private set; }
        public enum VerticalAnchor
        {
            Top,
            Bottom
        }


        public static VerticalAnchor NavbarAnchor = VerticalAnchor.Top;

        private static readonly Vector2 NAVBAR_DIMENSIONS = new(1020f, 35f);

        public AdminMenuPanel(UIBase owner) : base(owner)
        {
            Instance = this;
        }

        public override string Name => "BloodyShopMenu";

        public override int MinWidth => 200;

        public override int MinHeight => 50;

        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 1f);

        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 1f);

        public override Vector2 DefaultPosition => new Vector2(0 - MinWidth / 2, Screen.height + 40);

        public override bool CanDragAndResize => false;


        public static RectTransform NavBarRect { get; private set; }

        public GameObject NavbarTabButtonHolder { get; private set; }
        
        public ButtonRef shopBtn { get; private set; }
        public ButtonRef addItemBtn { get; private set; }
        public ButtonRef openCloseBtn { get; private set; }
        public ButtonRef deleteItemBtn { get; private set; }

        public Text stateTXT { get; private set; }
        public bool active { get; private set; }

        private int iconWH = 45;

        protected override void ConstructPanelContent()
        {

            active = true;
            TitleBar.GetComponent<Image>().color = new Color(1, 1, 1, 0f);
            UIRoot.GetComponent<Image>().color = new Color(233, 237, 240, 0.2f);
            GameObject navbarPanel = UIFactory.CreateUIObject("MainNavbar", UIRoot);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(navbarPanel, false, false, true, true, 5, 4, 4, 4, 4, TextAnchor.MiddleCenter);
            NavBarRect = navbarPanel.GetComponent<RectTransform>();
            NavBarRect.pivot = new Vector2(0.5f, 1f);

            SetNavBarAnchor();

            // Shop BTN
            shopBtn = UIFactory.CreateButton(navbarPanel, "ShopButton", "", new Color(1, 1, 1, 1));
            UIFactory.SetLayoutElement(shopBtn.Component.gameObject, minWidth: iconWH, minHeight: iconWH, preferredWidth: iconWH, preferredHeight: iconWH, flexibleWidth: 0, flexibleHeight: 0);

            configShopButtonPanel();

            // CONFIG BTN
            addItemBtn = UIFactory.CreateButton(navbarPanel, "AdminButton", "", new Color(1, 1, 1, 1));
            UIFactory.SetLayoutElement(addItemBtn.Component.gameObject, minWidth: iconWH, minHeight: iconWH, preferredWidth: iconWH, preferredHeight: iconWH, flexibleWidth: 0, flexibleHeight: 0);
            addItemBtn.OnClick += OpenAddItemPanel;
            var icon = SpritesUtil.LoadPNGTOSprite(Properties.Resources.config);
            var iconBtn = addItemBtn.GameObject.GetComponent<Image>();
            iconBtn.sprite = icon;
            iconBtn.color = new Color(1, 1, 1, 1);

            // OPEN / CLOSE BTN
            openCloseBtn = UIFactory.CreateButton(navbarPanel, "CloseStoreBtn", "", new Color(1, 1, 1, 1));
            UIFactory.SetLayoutElement(openCloseBtn.Component.gameObject, minWidth: iconWH, minHeight: iconWH, preferredWidth: iconWH, preferredHeight: iconWH, flexibleWidth: 0, flexibleHeight: 0);
            if (ClientDB.shopOpen)
            {
                icon = SpritesUtil.LoadPNGTOSprite(Properties.Resources.open);
                iconBtn = openCloseBtn.GameObject.GetComponent<Image>();
                iconBtn.sprite = icon;
                iconBtn.color = new Color(1, 1, 1, 1);
            }
            else
            {
                icon = SpritesUtil.LoadPNGTOSprite(Properties.Resources.close);
                iconBtn = openCloseBtn.GameObject.GetComponent<Image>();
                iconBtn.sprite = icon;
                iconBtn.color = new Color(1, 1, 1, 1);
            }
            openCloseBtn.OnClick += OpenCloseAction;

        }

        public static void SetNavBarAnchor()
        {
            switch (NavbarAnchor)
            {
                case VerticalAnchor.Top:
                    NavBarRect.anchorMin = new Vector2(0.5f, 1f);
                    NavBarRect.anchorMax = new Vector2(0.5f, 1f);
                    NavBarRect.anchoredPosition = new Vector2(NavBarRect.anchoredPosition.x, 40);
                    NavBarRect.sizeDelta = NAVBAR_DIMENSIONS;
                    break;

                case VerticalAnchor.Bottom:
                    NavBarRect.anchorMin = new Vector2(0.5f, 0f);
                    NavBarRect.anchorMax = new Vector2(0.5f, 0f);
                    NavBarRect.anchoredPosition = new Vector2(NavBarRect.anchoredPosition.x, 35);
                    NavBarRect.sizeDelta = NAVBAR_DIMENSIONS;
                    break;
            }
        }

        public void closeShop()
        {
            configShopButtonPanel();
        }

        public void openShop()
        {

            configShopButtonPanel();
        }

        private static void OpenShopPanel()
        {
            //Sound.Play(Properties.Resources.click_x);
            UIManager.ShowShopPanel();
        }

        private static void OpenDeletePanel()
        {
            //UIManager.ShowDeletePanel();
        }

        private static void OpenAddItemPanel()
        {
            //Sound.Play(Properties.Resources.click_x);
            UIManager.ShowPanelConfigPanel();
        }

        private void configShopButtonPanel()
        {

            if (!ClientDB.shopOpen)
            {
                var icon = SpritesUtil.LoadPNGTOSprite(Properties.Resources.shop_close);
                var iconBtn = shopBtn.GameObject.GetComponent<Image>();
                iconBtn.sprite = icon;
                iconBtn.color = new Color(1, 1, 1, 1);
               
                shopBtn.OnClick -= OpenShopPanel;
            }
            else
            {
                var icon = SpritesUtil.LoadPNGTOSprite(Properties.Resources.shop_open);
                var iconBtn = shopBtn.GameObject.GetComponent<Image>();
                iconBtn.sprite = icon;
                iconBtn.color = new Color(1, 1, 1, 1);
                shopBtn.OnClick += OpenShopPanel;
            }

        }

        private void OpenCloseAction()
        {

            if (ClientDB.shopOpen)
            {
                ClientDB.shopOpen = false;
                var icon = SpritesUtil.LoadPNGTOSprite(Properties.Resources.open);
                var iconBtn = openCloseBtn.GameObject.GetComponent<Image>();
                iconBtn.sprite = icon;
                iconBtn.color = new Color(1, 1, 1, 1);

                ClientCloseMessageAction.Send();
            }
            else
            {
                ClientDB.shopOpen = true;
                var icon = SpritesUtil.LoadPNGTOSprite(Properties.Resources.close);
                var iconBtn = openCloseBtn.GameObject.GetComponent<Image>();
                iconBtn.sprite = icon;
                iconBtn.color = new Color(1, 1, 1, 1);

                ClientOpenMessageAction.Send();
            }

        }

    }
}
