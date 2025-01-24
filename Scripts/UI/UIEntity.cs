using UnityEngine;

public enum UIOn
{
    LeftScreen,
    RightScreen,
}

public abstract class UIEntity : MonoBehaviour
{
    public UIOn UIOn;
    protected RectTransform rectTrans;
    private bool isDestroy;

    public bool IsDestroy
    {
        get { return isDestroy; }
        set { isDestroy = value; }
    }
    public bool IsShow;

    protected virtual void Awake()
    {
        rectTrans = this.gameObject.GetComponent<RectTransform>();
    }

    public void AutoPutOnParent()
    {
        if(UIOn.LeftScreen == UIOn)
        {
            transform.SetParent(Main.Instance.MainCanvas.LeftScreen);
            transform.SetAsLastSibling();
        }
        else
        {
            transform.SetParent(Main.Instance.MainCanvas.RightScreen);
            transform.SetAsLastSibling();
        }
    }

    public virtual void Init()
    {
        IsDestroy = false;
    }

    public abstract void LoadData();

    // UIË¢ÐÂ
    public abstract void RefreshUI();

    public virtual void Show()
    {
        this.gameObject.SetActive(true);
        IsShow = true;
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
        IsShow = false;
    }

    public virtual void Destroy()
    {
        if (IsShow) Hide();
        isDestroy = true;
        //PoolManager.GetUIPool().UnSpawn(gameObject, name);
        GameObject.Destroy(this.gameObject);
    }
}
