using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public enum UIOn
{
    LeftScreen,
    RightScreen,
    Whole,
    Other,
}

public abstract class UIEntity : MonoBehaviour
{
    public UIOn UIOn;
    protected RectTransform rectTrans;
    private bool _isDestroy;
    private Action _onClose;

    public bool IsDestroy
    {
        get { return _isDestroy; }
        set { _isDestroy = value; }
    }

    protected virtual void Awake()
    {
        rectTrans = this.gameObject.GetComponent<RectTransform>();
    }

    public void AutoPutOnParent()
    {
        // 暂时没用
        if(UIOn.LeftScreen == UIOn)
        {
            transform.SetParent(Main.Instance.MainCanvas.LeftScreen);
            transform.SetAsLastSibling();
        }
        else if(UIOn.RightScreen == UIOn)
        {
            transform.SetParent(Main.Instance.MainCanvas.RightScreen);
            transform.SetAsLastSibling();
        }
        else if(UIOn.Other == UIOn)
        {
            transform.SetParent(Main.Instance.MainCanvas.WholeScreen);
            transform.SetAsLastSibling();
        }
        else
        {
            return;
        }
    }

    public void Init(Action closeCB = null)
    {
        _onClose = closeCB;
        IsDestroy = false;
        OnInit();
    }

    protected virtual void OnInit() { }

    protected virtual void OnClose() { }

    public abstract void LoadData();

    // UI刷新
    public abstract void RefreshUI();

    public virtual void Close()
    {
        this.gameObject.SetActive(false);
        _onClose?.Invoke();
        OnClose();
    }
}
