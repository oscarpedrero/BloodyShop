using BloodyShop.Client.DB;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace BloodyShop.Client.UI.Panels
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

        public override string Name => "BloodyShopMenu";

        public override int MinWidth => 350;

        public override int MinHeight => 35;

        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 1f);

        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 1f);

        public override Vector2 DefaultPosition => new Vector2(0 - (MinWidth/2), Screen.height);

        public override bool CanDragAndResize => false;


        public static RectTransform NavBarRect { get; private set; }

        public GameObject NavbarTabButtonHolder { get; private set; }
        public ButtonRef closeBtn { get; private set; }
        public ButtonRef shopBtn { get; private set; }
        public ButtonRef adminBtn { get; private set; }
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

            // Title TXT
            string titleTxt = $"<i><color=white>{ClientDB.shopName}</color></i>";
            Text title = UIFactory.CreateLabel(navbarPanel, "Title", titleTxt, TextAnchor.MiddleLeft, default, true, 14);
            UIFactory.SetLayoutElement(title.gameObject, minWidth: 80, flexibleWidth: 0);

            //spacer
            GameObject spacerOne = UIFactory.CreateUIObject("Spacer", navbarPanel);
            UIFactory.SetLayoutElement(spacerOne, minWidth: 10);

            // Shop BTN
            shopBtn = UIFactory.CreateButton(navbarPanel, "ShopButton", "Shop");
            UIFactory.SetLayoutElement(shopBtn.Component.gameObject, minHeight: 25, minWidth: 60, flexibleWidth: 0);
            RuntimeHelper.SetColorBlock(shopBtn.Component, new Color(0 / 255f, 153 / 255f, 255 / 255f),
                new Color(0 / 255f, 138 / 255f, 230 / 255f), new Color(51 / 255f, 173 / 255f, 255 / 255f));
            shopBtn.OnClick += OpenShopPanel;

            if (!ClientDB.shopOpen)
            {
                shopBtn.Enabled = false;
                shopBtn.OnClick -= OpenShopPanel;
            }

            if (ClientDB.userModel.IsAdmin)
            {
                // Admin BTN
                adminBtn = UIFactory.CreateButton(navbarPanel, "AdminButton", "Admin");
                UIFactory.SetLayoutElement(adminBtn.Component.gameObject, minHeight: 25, minWidth: 60, flexibleWidth: 0);
                RuntimeHelper.SetColorBlock(adminBtn.Component, new Color(51 / 255f, 153 / 255f, 51 / 255f),
                   new Color(45 / 255f, 134 / 255f, 45 / 255f), new Color(64 / 255f, 191 / 255f, 64 / 255f));
                adminBtn.OnClick += OpenAdminPanel;

                //spacer
                GameObject spacerTwo = UIFactory.CreateUIObject("Spacer", navbarPanel);
                UIFactory.SetLayoutElement(spacerTwo, minWidth: 30);

            } else
            {
                //spacer
                GameObject spacerTwo = UIFactory.CreateUIObject("Spacer", navbarPanel);
                UIFactory.SetLayoutElement(spacerTwo, minWidth: 90);
            }

            

            // close BTNH
            closeBtn = UIFactory.CreateButton(navbarPanel, "CloseButton", "Close");
            UIFactory.SetLayoutElement(closeBtn.Component.gameObject, minHeight: 25, minWidth: 60, flexibleWidth: 0);
            RuntimeHelper.SetColorBlock(closeBtn.Component, new Color(0.63f, 0.32f, 0.31f),
                new Color(0.81f, 0.25f, 0.2f), new Color(0.6f, 0.18f, 0.16f));
            closeBtn.OnClick += CloseMenuPanel;

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
            shopBtn.Enabled = false;
            shopBtn.OnClick -= OpenShopPanel;
        }

        public void openShop()
        {
            shopBtn.Enabled = true;
            shopBtn.OnClick += OpenShopPanel;
        }

        private static void OpenShopPanel()
        {
            UIManager.OpenShopPanel();
        }

        private static void OpenAdminPanel()
        {
            UIManager.OpenAdminPanel();
        }

        private static void CloseMenuPanel()
        {
            UIManager.CloseMenuPanel();
        }

    }
}
