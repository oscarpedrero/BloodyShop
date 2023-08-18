using BloodyShop.Client.DB;
using BloodyShop.Client.Utils;
using BloodyShop.Utils;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace BloodyShop.Client.UI.Panels.User
{

    public class MenuPanel : UniverseLib.UI.Panels.PanelBase
    {

        public static MenuPanel Instance { get; private set; }
        public enum VerticalAnchor
        {
            Top,
            Bottom
        }


        public static VerticalAnchor NavbarAnchor = VerticalAnchor.Top;

        private static readonly Vector2 NAVBAR_DIMENSIONS = new(1020f, 35f);

        public MenuPanel(UIBase owner) : base(owner)
        {
            Instance = this;
        }

        public override string Name => "Bloody Shop Menu";

        public override int MinWidth => 70;

        public override int MinHeight => 50;

        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 1f);

        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 1f);

        public override Vector2 DefaultPosition => new Vector2(0 - MinWidth / 2, Screen.height);

        public override bool CanDragAndResize => false;


        public static RectTransform NavBarRect { get; private set; }

        public GameObject NavbarTabButtonHolder { get; private set; }
        public ButtonRef closeBtn { get; private set; }
        public ButtonRef shopBtn { get; private set; }
        public ButtonRef adminBtn { get; private set; }
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
            navbarPanel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
            NavBarRect = navbarPanel.GetComponent<RectTransform>();
            NavBarRect.pivot = new Vector2(0.5f, 1f);

            SetNavBarAnchor();

            // Shop BTN
            shopBtn = UIFactory.CreateButton(navbarPanel, "ShopButton", "Shop");
            UIFactory.SetLayoutElement(shopBtn.Component.gameObject, minWidth: iconWH, minHeight: iconWH, preferredWidth: iconWH, preferredHeight: iconWH, flexibleWidth: 0, flexibleHeight: 0);

            configShopButtonPanel();

        }

        public static void SetNavBarAnchor()
        {
            switch (NavbarAnchor)
            {
                case VerticalAnchor.Top:
                    NavBarRect.anchorMin = new Vector2(0.5f, 1f);
                    NavBarRect.anchorMax = new Vector2(0.5f, 1f);
                    NavBarRect.anchoredPosition = new Vector2(NavBarRect.anchoredPosition.x, 0);
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
            UIManager.ShowShopPanel();
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

    }
}
