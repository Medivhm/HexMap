using System.Collections;
using Tiles;
using UnityEngine;
using UnityEngine.UI;

public class UIFunction : MonoBehaviour
{
    public Text text;


    void Start()
    {
        StartCoroutine(GetFPS());
    }

    IEnumerator GetFPS()
    {
        while (true)
        {
            text.text = (Mathf.FloorToInt(1 / Time.deltaTime)).ToString();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public Text ChangeModeText;
    public void ChangeGameMode()
    {
        if(Main.Instance.Mode == GameMode.Play)
        {
            Main.Instance.Mode = GameMode.MoveMap;
            ChangeModeText.text = "“∆∂Ø";
        }
        else if(Main.Instance.Mode == GameMode.MoveMap)
        {
            Main.Instance.Mode = GameMode.Play;
            ChangeModeText.text = "”Œœ∑";
        }
    }

    public void ShowAllBorders()
    {
        GridManager.Instance.ShowAllBorders();
    }

    public void HideAllBorders()
    {
        GridManager.Instance.HideAllBorders();
    }

    public void ChangeToFollowPlayerMode()
    {
        Main.Instance.MainCameraController.FollowPlayer();
    }
}
