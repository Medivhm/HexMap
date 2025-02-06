//using System.Collections.Generic;
//using UnityEngine;
//using YooAsset;
//using System.Collections;
//using UnityEngine.Events;

//public class UIManager : MonoSingleton<UIManager>
//{
//    [System.Serializable]
//    public class UIInfo
//    {
//        public string uiName; // UI名称
//        public string assetPath; // YooAsset资源路径
//        public int sortingOrderBase = 0; // UI层级基础值
//        public bool isPersistent = false; // 是否常驻内存
//    }

//    [SerializeField] private List<UIInfo> uiConfigs = new List<UIInfo>(); // UI配置列表
//    [SerializeField] private Transform[] layerParents; // UI层级父节点（0:背景层，1:普通层，2:弹窗层）

//    private Dictionary<string, UIInfo> _uiDictionary = new Dictionary<string, UIInfo>(); // UI配置字典
//    private Dictionary<string, GameObject> _activePanels = new Dictionary<string, GameObject>(); // 当前激活的UI面板
//    private Dictionary<string, AssetHandle> _loadedHandles = new Dictionary<string, AssetHandle>(); // 已加载的资源句柄
//    private UIPanelStack<GameObject> _panelStack = new UIPanelStack<GameObject>(); // UI面板栈（用于返回功能）

//    private void Awake()
//    {
//        Initialize();
//        DontDestroyOnLoad(gameObject);
//    }

//    private void Initialize()
//    {
//        // 初始化UI配置字典
//        foreach (var config in uiConfigs)
//        {
//            _uiDictionary[config.uiName] = config;
//        }
//    }

//    public UIEntity OpenUI(string uiName)
//    {
//        // 检查是否已经激活
//        if (_activePanels.TryGetValue(uiName, out GameObject existingPanel))
//        {
//            existingPanel.SetActive(true);
//            BringToTop(existingPanel);
//            return existingPanel.GetComponent<UIEntity>();
//        }

//        // 检查是否在配置中
//        if (!_uiDictionary.TryGetValue(uiName, out UIInfo uiInfo))
//        {
//            Debug.LogError($"UI {uiName} not found in config!");
//            return null;
//        }

//        // 检查是否已经加载过
//        if (_loadedHandles.TryGetValue(uiInfo.uiName, out AssetHandle handle))
//        {
//            if (handle.Status == EOperationStatus.Succeed)
//            {
//                return InstantiateUI(uiInfo, handle.AssetObject as GameObject).GetComponent<UIEntity>();
//            }
//            else
//            {
//                Debug.LogError($"Failed to load UI: {uiInfo.uiName}");
//                return null;
//            }
//        }

//        // 同步加载
//        var loadHandle = YooAssets.LoadAssetSync<GameObject>(uiInfo.assetPath);
//        _loadedHandles[uiInfo.uiName] = loadHandle;

//        if (loadHandle.Status == EOperationStatus.Succeed)
//        {
//            return InstantiateUI(uiInfo, loadHandle.AssetObject as GameObject).GetComponent<UIEntity>();
//        }
//        else
//        {
//            Debug.LogError($"Failed to load UI: {uiInfo.uiName}");
//            loadHandle.Release();
//            return null;
//        }
//    }

//    private GameObject InstantiateUI(UIInfo uiInfo, GameObject prefab)
//    {
//        // 确定UI层级
//        int layerIndex = Mathf.Clamp(uiInfo.sortingOrderBase / 1000, 0, layerParents.Length - 1);
//        GameObject uiInstance = Instantiate(prefab, layerParents[layerIndex]);

//        // 设置Canvas排序
//        Canvas canvas = uiInstance.GetComponent<Canvas>();
//        if (canvas != null)
//        {
//            canvas.sortingOrder = uiInfo.sortingOrderBase + _panelStack.Count * 10;
//        }

//        // 记录UI实例
//        _activePanels[uiInfo.uiName] = uiInstance;
//        _panelStack.Push(uiInstance);

//        // 初始化UI面板
//        UIEntity panel = uiInstance.GetComponent<UIEntity>();
//        if (panel != null)
//        {
//            panel.Init(() => CloseUI(uiInfo.uiName));
//        }

//        return uiInstance;
//    }

//    // 异步打开UI
//    public void OpenUIAsync(string uiName, UnityAction<GameObject> onComplete = null, UnityAction<float> onProgress = null)
//    {
//        if (_activePanels.TryGetValue(uiName, out GameObject existingPanel))
//        {
//            // 如果UI已经加载过，直接激活
//            existingPanel.SetActive(true);
//            BringToTop(existingPanel);
//            onComplete?.Invoke(existingPanel);
//            return;
//        }

//        if (!_uiDictionary.TryGetValue(uiName, out UIInfo uiInfo))
//        {
//            Debug.LogError($"UI {uiName} not found in config!");
//            return;
//        }

//        StartCoroutine(LoadUIProcess(uiInfo, onComplete, onProgress));
//    }

//    private IEnumerator LoadUIProcess(UIInfo uiInfo, UnityAction<GameObject> onComplete, UnityAction<float> onProgress)
//    {
//        // 如果已经加载过
//        if (_loadedHandles.TryGetValue(uiInfo.uiName, out AssetHandle handle))
//        {
//            if (handle.IsDone)
//            {
//                InstantiateUI(uiInfo, handle.AssetObject as GameObject, onComplete);
//            }
//            yield break;
//        }

//        // 使用YooAsset异步加载资源
//        var loadHandle = YooAssets.LoadAssetAsync<GameObject>(uiInfo.assetPath);
//        _loadedHandles[uiInfo.uiName] = loadHandle;

//        while (!loadHandle.IsDone)
//        {
//            onProgress?.Invoke(loadHandle.Progress);
//            yield return null;
//        }

//        if (loadHandle.Status == EOperationStatus.Succeed)
//        {
//            InstantiateUI(uiInfo, loadHandle.AssetObject as GameObject, onComplete);
//        }
//        else
//        {
//            Debug.LogError($"Failed to load UI: {uiInfo.uiName}");
//            loadHandle.Dispose();
//        }
//    }

//    private void InstantiateUI(UIInfo uiInfo, GameObject prefab, UnityAction<GameObject> onComplete)
//    {
//        // 确定UI层级
//        int layerIndex = Mathf.Clamp(uiInfo.sortingOrderBase / 1000, 0, layerParents.Length - 1);
//        GameObject uiInstance = Instantiate(prefab, layerParents[layerIndex]);

//        // 设置Canvas排序
//        Canvas canvas = uiInstance.GetComponent<Canvas>();
//        if (canvas != null)
//        {
//            canvas.sortingOrder = uiInfo.sortingOrderBase + _panelStack.Count * 10;
//        }

//        // 记录UI实例
//        _activePanels[uiInfo.uiName] = uiInstance;
//        _panelStack.Push(uiInstance);

//        // 初始化UI面板
//        UIEntity panel = uiInstance.GetComponent<UIEntity>();
//        if (panel != null)
//        {
//            panel.Init(() => CloseUI(uiInfo.uiName));
//        }

//        onComplete?.Invoke(uiInstance);
//    }

//    // 关闭UI
//    public void CloseUI(string uiName)
//    {
//        if (_activePanels.TryGetValue(uiName, out GameObject panel))
//        {
//            if (_panelStack.Count > 0 && _panelStack.Peek() == panel)
//            {
//                _panelStack.Pop();
//            }

//            // 非常驻UI立即销毁
//            if (!_uiDictionary[uiName].isPersistent)
//            {
//                Destroy(panel);
//                _activePanels.Remove(uiName);
//                ReleaseResource(uiName);
//            }
//            else
//            {
//                panel.SetActive(false);
//            }

//            // 调整栈内其他面板的层级
//            UpdateSortingOrders();
//        }
//    }

//    // 释放资源
//    private void ReleaseResource(string uiName)
//    {
//        if (_loadedHandles.TryGetValue(uiName, out AssetHandle handle))
//        {
//            handle.Dispose();
//            _loadedHandles.Remove(uiName);
//        }
//    }

//    // 将UI置顶
//    private void BringToTop(GameObject panel)
//    {
//        _panelStack.Remove(panel); // 需要自定义扩展Stack的Remove方法
//        _panelStack.Push(panel);
//        UpdateSortingOrders();
//    }

//    // 更新UI层级排序
//    private void UpdateSortingOrders()
//    {
//        int index = 0;
//        foreach (var panel in _panelStack)
//        {
//            Canvas canvas = panel.GetComponent<Canvas>();
//            if (canvas != null)
//            {
//                canvas.sortingOrder = _uiDictionary[panel.name].sortingOrderBase + index * 10;
//            }
//            index++;
//        }
//    }

//    // 清除非持久化UI
//    public void ClearNonPersistentUI()
//    {
//        List<string> toRemove = new List<string>();
//        foreach (var kvp in _activePanels)
//        {
//            if (!_uiDictionary[kvp.Key].isPersistent)
//            {
//                Destroy(kvp.Value);
//                ReleaseResource(kvp.Key);
//                toRemove.Add(kvp.Key);
//            }
//        }
//        foreach (var key in toRemove) _activePanels.Remove(key);
//    }

//    private void OnDestroy()
//    {
//        // 释放所有资源句柄
//        foreach (var handle in _loadedHandles.Values)
//        {
//            handle.Dispose();
//        }
//        _loadedHandles.Clear();
//    }
//}