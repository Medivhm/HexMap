
using Tiles;
using UnityEngine;

public class Player
{
    public bool Is2D;
    public bool Is3D => !Is2D;

    Unit _unit;
    public Unit Unit => _unit;

    #region ÉèÖÃunit
    public Player(Unit unit)
    {
        _unit = unit;
        SetUnit3DOr2D();
    }

    public void SetUnit(Unit unit)
    {
        _unit = unit;
        SetUnit3DOr2D();
    }

    private void SetUnit3DOr2D()
    {
        if (_unit is Unit2D)
            Is2D = true;
    }
    #endregion

    public bool IsInScreen()
    {
        return !IsOutOfScreen();
    }

    public bool IsOutOfScreen()
    {
        Vector3 characterScreenPos = Camera.main.WorldToScreenPoint(_unit.transform.position);
        return characterScreenPos.x > Screen.width || characterScreenPos.x < 0 ||
               characterScreenPos.y > Screen.height || characterScreenPos.y < 0;
    }
}