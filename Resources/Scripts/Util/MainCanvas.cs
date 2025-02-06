using System.Collections;
using Tiles;
using UnityEngine;
using UnityEngine.UI;

public partial class MainCanvas : MonoBehaviour
{
    // main game logic
    public Transform LeftScreen, RightScreen, WholeScreen;

    private void Start()
    {
        LeftScreen = transform.Find("LeftScreen");
        RightScreen = transform.Find("RightScreen");
        WholeScreen = transform.Find("WholeScreen");
        Main.Instance.MainCanvas = this.GetComponent<MainCanvas>();
    }

    private void OnDestroy()
    {
        Main.Instance.MainCanvas = null;
    }
}


public partial class MainCanvas : MonoBehaviour
{
    // All Test
    public Text text;


    void Awake()
    {
        StartCoroutine(GetFPS());
    }

    IEnumerator GetFPS()
    {
        while (true)
        {
            text.text = (Mathf.FloorToInt(1 / Time.deltaTime)).ToString();
            //yield return new WaitForSeconds(0.2f);
            yield return null;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowAllBorders()
    {
        GridManager.Instance.ShowAllBorders();
    }

    public void HideAllBorders()
    {
        GridManager.Instance.HideAllBorders();
    }

    public void ChangeToFollowMode()
    {
        Main.Instance.MainCameraController.MoveToPlayer(() =>
        {
            Main.Instance.MainCameraController.FollowPlayer();
        });
    }

    public void UITest()
    {
        //UIManager.Instance.OpenUIAsync("TestAssetUI");
    }
}
