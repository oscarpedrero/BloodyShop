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
using UniverseLib;

namespace BloodyShop.Client.UI.Panels.Admin
{
    public class PanelConfig : UniverseLib.UI.Panels.PanelBase
    {

        public static PanelConfig Instance { get; private set; }

        public override string Name => "Config Shop";

        public override bool CanDragAndResize => true;

        public override int MinWidth => 880;
        public override int MinHeight => 535;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 1f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 1f);
        public override Vector2 DefaultPosition => new Vector2(0 - MinWidth / 2 - 680, 0 + MinWidth / 2);

        public static float CurrentPanelWidth => Instance.Rect.rect.width;
        public static float CurrentPanelHeight => Instance.Rect.rect.height;

        public static bool active = false;

        public int SelectedTab = 0;
        private readonly List<UIModel> tabPages = new();
        private readonly List<ButtonRef> tabButtons = new();

        public static AddItemPanel addItemPanel;
        public static DeleteItemPanel deletePanel;

        public PanelConfig(UIBase owner) : base(owner)
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

            GameObject tabGroup = UIFactory.CreateHorizontalGroup(ContentRoot, "TabBar", true, true, true, true, 2, new Vector4(2, 2, 2, 2));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 50, flexibleHeight: 0);

            // Add Item
            addItemPanel = new AddItemPanel(this);
            addItemPanel.ConstructUI(ContentRoot);
            tabPages.Add(addItemPanel);

            // Delete Item
            deletePanel = new DeleteItemPanel(this);
            deletePanel.ConstructUI(ContentRoot);
            tabPages.Add(deletePanel);

            

            // set up tabs
            AddTabButton(tabGroup, "Add Item");
            AddTabButton(tabGroup, "Delete Item");

            SetTab(0);

            SetActive(true);

        }

        private void RefreshAction()
        {
            if (SelectedTab == 1)
                deletePanel.RefreshAction();
        }

        public DeleteItemPanel GetActivePanel()
        {
            return deletePanel;
        }

        public DeleteItemPanel GetCurrencyPanel()
        {
            return deletePanel;
        }

        public void SetTab(int tabIndex)
        {
            if (SelectedTab != -1)
                DisableTab(SelectedTab);

            UIModel content = tabPages[tabIndex];
            content.SetActive(true);

            ButtonRef button = tabButtons[tabIndex];
            RuntimeHelper.SetColorBlock(button.Component, UniversalUI.EnabledButtonColor, UniversalUI.EnabledButtonColor * 1.2f);

            SelectedTab = tabIndex;
        }

        private void AddTabButton(GameObject tabGroup, string label)
        {
            ButtonRef button = UIFactory.CreateButton(tabGroup, $"Button_{label}", label);

            int idx = tabButtons.Count;
            button.OnClick += () => { SetTab(idx); };

            tabButtons.Add(button);

            DisableTab(tabButtons.Count - 1);
        }

        private void DisableTab(int tabIndex)
        {
            tabPages[tabIndex].SetActive(false);
            RuntimeHelper.SetColorBlock(tabButtons[tabIndex].Component, UniversalUI.DisabledButtonColor, UniversalUI.DisabledButtonColor * 1.2f);
        }


    }
}
