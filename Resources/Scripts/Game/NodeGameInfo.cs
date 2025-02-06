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
    private HexType _hexType;

    public bool HasEvent => _playerInEvent != null;
    public HexType HexType => _hexType;

    public NodeGameInfo(HexType type)
    {
        this._hexType = type;
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