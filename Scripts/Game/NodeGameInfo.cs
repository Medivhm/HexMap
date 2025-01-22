using Tiles;
using System;
using UnityEngine;

public class NodeGameInfo
{
    private Sprite FrontArt;
    private Sprite BackArt;
    private Sprite Road;
    private Unit itemIcon;
    private Unit[] others;

    private Action _playerInEvent;
    private GridType _gridType;

    public bool HasEvent => _playerInEvent != null;
    public GridType GridType => _gridType;

    public NodeGameInfo(GridType type)
    {
        this._gridType = type;
    }

    public void SetPlayerInEvent(Action a)
    {
        if (HasEvent) return;

        _playerInEvent = a;
    }

    public void ClearPlayerInEvent()
    {
        _playerInEvent = null;
    }
}