
using Tiles;
using UnityEngine;

public class Player
{
    Unit _unit;
    public Unit Unit => _unit;

    public Player(Unit unit)
    {
        _unit = unit;
    }

    public bool IsInScreen()
    {
        return !IsOutOfScreen();
    }

    public bool IsOutOfScreen()
    {
        Vector3 characterScreenPos = Camera.main.WorldToScreenPoint(Main.Instance.MainPlayer.Unit.transform.position);
        return characterScreenPos.x > Screen.width || characterScreenPos.x < 0 ||
               characterScreenPos.y > Screen.width || characterScreenPos.y < 0;
    }
}