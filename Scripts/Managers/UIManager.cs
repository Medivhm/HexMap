using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    class UIManager : Singleton<UIManager>
    {
        static Dictionary<string, UIEntity> UIs;

        public static bool HasUI()
        {
            return UIs.Count > 0;
        }

        public void Init()
        {
            UIs = new Dictionary<string, UIEntity>();
            InitEscPress();
        }

        void InitEscPress()
        {

        }

        public static void Add(string uiName, UIEntity ui)
        {
            if (!HasUI(uiName))
            {
                UIs.Add(uiName, ui);
            }
        }

        public static void DestroyUI(UIEntity ui)
        {
            string name = ui.name;
            DestroyUI(name);
        }

        public static void DestroyUI(string uiName)
        {
            UIEntity ui = Get(uiName);
            if (ui)
            {
                ui.Destroy();
                UIs.Remove(uiName);
            }
        }

        public static UIEntity Get(string uiName)
        {
            UIEntity ui = null;
            UIs.TryGetValue(uiName, out ui);
            return ui;
        }

        public static UIEntity Show(string uiName)
        {
            UIEntity ui = null;
            UIs.TryGetValue(uiName, out ui);
            if (ui.IsNotNull() && ui.IsShow)
            {
                return ui;
            }

            if (ui.IsNull())
            {
                ui = SpawnUI(uiName);
                ui.AutoPutOnParent();
                ui.Init();
                UIs.Add(uiName, ui);
            }
            ui.RefreshUI();
            ui.Show();
            return ui;
        }

        public static UIEntity SpawnUI(string uiName)
        {
            return LoadTool.LoadUI(uiName);
        }

        public static void Hide(string uiName)
        {
            if (!HasUI(uiName)) return;

            UIs[uiName].Hide();
        }

        public static bool HasUI(string uiName)
        {
            if (UIs.ContainsKey(uiName))
            {
                return true;
            }
            return false;
        }

        public void Clear()
        {
            UIs.Clear();
        }
    }
}
