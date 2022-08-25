using BloodyShop.Client.DB;
using BloodyShop.Client.Network;
using BloodyShop.Utils;
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

        public override int MinWidth => 800;

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

        protected override void ConstructPanelContent()
        {

            active = true;
            GameObject navbarPanel = UIFactory.CreateUIObject("MainNavbar", UIRoot);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(navbarPanel, false, false, true, true, 5, 4, 4, 4, 4, TextAnchor.MiddleCenter);
            navbarPanel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
            NavBarRect = navbarPanel.GetComponent<RectTransform>();
            NavBarRect.pivot = new Vector2(0.5f, 1f);

            SetNavBarAnchor();

            // SHOP NAME TXT
            var shopNameTXT = UIFactory.CreateLabel(navbarPanel, "Title", $"<i>{FontColorChat.White(ClientDB.shopName)}</i>", TextAnchor.MiddleLeft, default, true, 14);
            UIFactory.SetLayoutElement(shopNameTXT.gameObject, minWidth: 160, minHeight: 40, preferredWidth: 160, preferredHeight: 40, flexibleWidth: 0, flexibleHeight: 0);

            // Shop BTN
            shopBtn = UIFactory.CreateButton(navbarPanel, "ShopButton", "Shop");
            UIFactory.SetLayoutElement(shopBtn.Component.gameObject, minWidth: 100, minHeight: 40, preferredWidth: 100, preferredHeight: 40, flexibleWidth: 0, flexibleHeight: 0);

            configShopButtonPanel();

            var _separator = UIFactory.CreateHorizontalGroup(navbarPanel, "Separator", true, true, true, true, 4, default, new Color(0.1f, 0.1f, 0.1f));
            var fakeTXT = UIFactory.CreateLabel(_separator, "FakeTextt-","", TextAnchor.MiddleCenter);
            UIFactory.SetLayoutElement(fakeTXT.gameObject, minWidth: 2, minHeight: 40, flexibleHeight: 0, preferredHeight: 40, flexibleWidth: 0, preferredWidth: 2);

            //spacer
            GameObject spacer = UIFactory.CreateUIObject("Spacer", navbarPanel);
            UIFactory.SetLayoutElement(spacer, minWidth: 10);

            // SHOP NAME TXT
            var AdminTXT = UIFactory.CreateLabel(navbarPanel, "Title", $"<i>{FontColorChat.White("Admin Tools")}</i>", TextAnchor.MiddleLeft, default, true, 10);
            UIFactory.SetLayoutElement(AdminTXT.gameObject, minWidth: 60, minHeight: 40, preferredWidth: 60, preferredHeight: 40, flexibleWidth: 0, flexibleHeight: 0);

            // ADD ITEM BTN
            addItemBtn = UIFactory.CreateButton(navbarPanel, "AdminButton", "Add Item", new Color(186 / 255f, 74 / 255f, 0 / 255f));
            UIFactory.SetLayoutElement(addItemBtn.Component.gameObject, minWidth: 100, minHeight: 40, preferredWidth: 100, preferredHeight: 40, flexibleWidth: 0, flexibleHeight: 0);
            addItemBtn.OnClick += OpenAddItemPanel;

            // DELETE ITEM BTN
            deleteItemBtn = UIFactory.CreateButton(navbarPanel, "AdminButton", "Delete Item", new Color(203 / 255f, 67 / 255f, 53 / 255f));
            UIFactory.SetLayoutElement(deleteItemBtn.Component.gameObject, minWidth: 100, minHeight: 40, preferredWidth: 100, preferredHeight: 40, flexibleWidth: 0, flexibleHeight: 0);
            deleteItemBtn.OnClick += OpenDeletePanel;

            // OPEN / CLOSE BTN
            openCloseBtn = UIFactory.CreateButton(navbarPanel, "CloseStoreBtn", "Open Store", new Color(0.3f, 0.2f, 0.2f));
            UIFactory.SetLayoutElement(openCloseBtn.Component.gameObject, minWidth: 100, minHeight: 40, preferredWidth: 100, preferredHeight: 40, flexibleWidth: 0, flexibleHeight: 0);
            if (ClientDB.shopOpen)
            {
                openCloseBtn.Component.GetComponentInChildren<Text>().text = "Close Store";
                RuntimeHelper.SetColorBlock(openCloseBtn.Component,
                       new Color(33 / 255f, 47 / 255f, 60 / 255f),
                        new Color(40 / 255f, 55 / 255f, 71 / 255f),
                        new Color(93 / 255f, 109 / 255f, 126 / 255f));
            }
            else
            {
                openCloseBtn.Component.GetComponentInChildren<Text>().text = "Open Store";
                RuntimeHelper.SetColorBlock(openCloseBtn.Component,
                      new Color(17 / 255f, 122 / 255f, 101 / 255f),
                       new Color(19 / 255f, 141 / 255f, 117 / 255f),
                       new Color(22 / 255f, 160 / 255f, 133 / 255f));
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
            UIManager.ShowShopPanel();
        }

        private static void OpenDeletePanel()
        {
            UIManager.ShowDeletePanel();
        }

        private static void OpenAddItemPanel()
        {
            UIManager.ShowAddItemPanel();
        }

        private void configShopButtonPanel()
        {

            if (!ClientDB.shopOpen)
            {
                shopBtn.Component.GetComponentInChildren<Text>().text = "Shop Closed";
                RuntimeHelper.SetColorBlockAuto(shopBtn.Component,
                  new Color(84 / 255f, 153 / 255f, 199 / 255f)
                  );
                shopBtn.OnClick -= OpenShopPanel;
            }
            else
            {
                shopBtn.Component.GetComponentInChildren<Text>().text = "Shop Opened";
                RuntimeHelper.SetColorBlockAuto(shopBtn.Component,
                   new Color(36 / 255f, 113 / 255f, 163 / 255f)
                   );
                shopBtn.OnClick += OpenShopPanel;
            }

        }

        private void OpenCloseAction()
        {
            if (ClientDB.shopOpen)
            {
                ClientDB.shopOpen = false;
                openCloseBtn.Component.GetComponentInChildren<Text>().text = "Open Store";
                RuntimeHelper.SetColorBlock(openCloseBtn.Component,
                      new Color(17 / 255f, 122 / 255f, 101 / 255f),
                       new Color(19 / 255f, 141 / 255f, 117 / 255f),
                       new Color(22 / 255f, 160 / 255f, 133 / 255f));

                ClientCloseMessageAction.Send();
            }
            else
            {
                ClientDB.shopOpen = true;
                openCloseBtn.Component.GetComponentInChildren<Text>().text = "Close Store";
                RuntimeHelper.SetColorBlock(openCloseBtn.Component,
                       new Color(33 / 255f, 47 / 255f, 60 / 255f),
                        new Color(40 / 255f, 55 / 255f, 71 / 255f),
                        new Color(93 / 255f, 109 / 255f, 126 / 255f));

                ClientOpenMessageAction.Send();
            }

        }

    }
}
