using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GridType
{
    Playable = 0,   // 交互关卡
    Obstacle,       // 障碍
    Road,           // 可行走区域
}

//          5    /\   0
//          4   |  |  1
//          3    \/   2

public enum SidePos
{
    NE = 0,   // +3) % 6
    E,
    SE,
    SW,
    W,
    NW,
    None,
}

namespace Tiles {
    public partial class HexNode : MonoBehaviour{
        NodeGameInfo gameInfo;
        [SerializeField] private List<SidePos> passSide;

        public void AfterInit(GridType type)
        {
            this.name = string.Format("{0}_{1}", Coords.MapCoord.x, Coords.MapCoord.y);
            gameInfo = new NodeGameInfo(type);

            //Debug.LogFormat($"{this.Coords.MapCoord.x}+{this.Coords.MapCoord.y}");
        }

        public bool CanPass(SidePos side)
        {
            return passSide.Contains(side);
        }

        public SidePos SwapSide(SidePos side)
        {
            return (SidePos)(((int)side + 3) % 6);
        }

        // 判断是否相连（可通行）
        public bool IsConnect(HexNode other)
        {
            SidePos thisPos = OtherInWhere(other);
            return CanPass(thisPos) && other.CanPass(SwapSide(thisPos));
        }

        public SidePos OtherInWhere(HexNode other)
        {
            float a = this.Coords.MapCoord.x - other.Coords.MapCoord.x;
            float b = this.Coords.MapCoord.y - other.Coords.MapCoord.y;
            if (a == 0 && b == -1)
            {
                return SidePos.NE;
            }
            else if (a == -1 && b == 0)
            {
                return SidePos.E;
            }
            else if (a == -1 && b == 1)
            {
                return SidePos.SE;
            }
            else if (a == 0 && b == 1)
            {
                return SidePos.SW;
            }
            else if (a == 1 && b == 0)
            {
                return SidePos.W;
            }
            else if (a == 1 && b == -1)
            {
                return SidePos.NW;
            }
            return SidePos.None;
        }


        public bool HasInEvent()
        {
            return gameInfo.HasEvent;
        }

        public void SetInEvent(Action a)
        {
            gameInfo.SetPlayerInEvent(a);
        }

        public List<HexNode> FindPath(HexNode toNode)
        {
            return Pathfinding.FindPath(this, toNode);
        }
    }

    public partial class HexNode : MonoBehaviour
    {
        private Material _lineMaterial;
        private SpriteRenderer _renderer;
        private LineRenderer _lineRenderer;
        private PolygonCollider2D _polygonCollider;

        public ICoords Coords;
        public float GetDistance(HexNode other) => Coords.GetDistance(other.Coords); // Helper to reduce noise in pathfinding
        public bool Walkable => true;
        private bool _selected;

        private void Awake()
        {
            _renderer = GetComponentInChildren<SpriteRenderer>();
            _lineRenderer = GetComponent<LineRenderer>();
            _polygonCollider = GetComponent<PolygonCollider2D>();
        }

        public void Init(GridType gridType, ICoords coords)
        {
            OnHoverTile += OnOnHoverTile;

            Coords = coords;
            transform.position = Coords.WorldPos;
            AfterInit(gridType);
        }

        public void SetLineMaterial(Material mater)
        {
            _lineMaterial = mater;
        }

        public void UpdateLineRenderer()
        {
            _lineRenderer.material = _lineMaterial;

            _lineRenderer.loop = true;
            _lineRenderer.startWidth = 0.05f;
            var points = _polygonCollider.points;
            _lineRenderer.positionCount = points.Length;
            for (int i = 0; i < points.Length; i++)
            {
                _lineRenderer.SetPosition(i, (Vector2)this.transform.position + points[i]);
            }
        }

        public void ShowBorder()
        {
            this._lineRenderer.enabled = true;
        }

        public void HideBorder()
        {
            this._lineRenderer.enabled = false;
        }

        private void OnMouseUp()
        {
            if (!Walkable) return;
            if (!TouchDuration.Instance.IsClick()) return;        // 如果是拖动，不执行

            OnHoverTile?.Invoke(this);
        }

        #region ClickEvent
        public static event Action<HexNode> OnHoverTile;
        private void OnEnable() => OnHoverTile += OnOnHoverTile;
        private void OnDisable() => OnHoverTile -= OnOnHoverTile;
        private void OnOnHoverTile(HexNode node) => _selected = node == this;
        #endregion

        #region Pathfinding
        public List<HexNode> Neighbors { get; protected set; }
        public HexNode Connection { get; private set; }
        public float G { get; private set; }
        public float H { get; private set; }
        public float F => G + H;

        public void CacheNeighbors()
        {
            Neighbors = GridManager.Instance.Tiles.Where(t => Coords.GetDistance(t.Value.Coords) == 1).Select(t => t.Value).Where(t => t.IsConnect(this)).ToList();
        }

        public void SetConnection(HexNode HexNode)
        {
            Connection = HexNode;
        }

        public void SetG(float g)
        {
            G = g;
            SetText();
        }

        public void SetH(float h)
        {
            H = h;
            SetText();
        }

        private void SetText()
        {
            if (_selected) return;
        }

        public void RevertTile()   // 点击地块回调
        {
        }

        #endregion
    }
}

public struct HexCoords : ICoords
{
    private readonly int _q;
    private readonly int _r;

    public HexCoords(int q, int r) {
        _q = q;
        _r = r;
        WorldPos = _q * new Vector2(Sqrt3, 0) + _r * new Vector2(Sqrt3 / 2, 1.5f);
    }

    public float GetDistance(ICoords other) => (this - (HexCoords)other).AxialLength();


    private static readonly float Sqrt3 = Mathf.Sqrt(3);

    public Vector2Int MapCoord
    {
        get
        {
            return new Vector2Int(_q, _r);
        }
    }
    public Vector3 WorldPos { get; set; }

    private int AxialLength() {
        if (_q == 0 && _r == 0) return 0;
        if (_q > 0 && _r >= 0) return _q + _r;
        if (_q <= 0 && _r > 0) return -_q < _r ? _r : -_q;
        if (_q < 0) return -_q - _r;
        return -_r > _q ? -_r : _q;
    }

    public static HexCoords operator -(HexCoords a, HexCoords b) {
        return new HexCoords(a._q - b._q, a._r - b._r);
    }
}

public interface ICoords
{
    public float GetDistance(ICoords other);
    public Vector2Int MapCoord { get; }
    public Vector3 WorldPos { get; set; }
}