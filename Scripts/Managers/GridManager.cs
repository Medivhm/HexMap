using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tiles {

    // 游戏内容
    public partial class GridManager : MonoSingleton<GridManager>
    {

        public Unit SpawnSpriteUnit(Vector2 pos, Sprite sprite)
        {
            return SpawnSpriteUnit(Tiles[pos], sprite);
        }

        public Unit SpawnSpriteUnit(HexNode node, Sprite sprite)
        {
            Unit unit = GameObject.Instantiate(_unitPrefab, node.Coords.WorldPos, Quaternion.Euler(Main.Instance.MainCameraController.EulerRotateX, 0f, 0f));
            unit.Init(sprite);
            unit.HexCoord = node.Coords.MapCoord;
            return unit;
        }

        private void OnTileHover(HexNode HexNode)
        {
            //if (Main.Instance.Mode != GameMode.Play) return;
            //if (Main.Instance.MainPlayer.Unit.IsMoving) return; // 移动不可改目标

            foreach (var t in Tiles.Values) t.RevertTile();

            List<HexNode> passNodes = Pathfinding.FindPath(GetTileByCoord(Main.Instance.MainPlayer.Unit.HexCoord), HexNode);
            if(passNodes != null && passNodes.Count > 0)
            {
                if (Main.Instance.MainPlayer.IsOutOfScreen())
                {
                    Main.Instance.MainCameraController.DisableTouch();
                    Main.Instance.MainCameraController.FollowPlayer(() =>
                    {
                        Main.Instance.MainCameraController.EnableTouch();
                        Main.Instance.MainPlayer.Unit.MoveTo(GetTileByCoord(Main.Instance.MainPlayer.Unit.HexCoord), HexNode, passNodes,
                            (_, _, _) =>
                            {
                                Main.Instance.MainCameraController.CameraMoveToHex(HexNode);
                            });
                    });
                }
                else
                {
                    Main.Instance.MainPlayer.Unit.MoveTo(GetTileByCoord(Main.Instance.MainPlayer.Unit.HexCoord), HexNode, passNodes,
                        (_, _, _) =>
                        {
                            //Main.Instance.MainCameraController.CameraMoveToHex(HexNode);
                        });
                }
            }
        }

        void SpawnUnits()
        {
            //foreach (var t in Tiles.Keys)
            //{
            //    SpawnSpriteUnit(t, _scriptableGrid.gos[GetRandom()]);
            //}
        }
    }


    public partial class GridManager : MonoSingleton<GridManager>
    {
        [SerializeField] private bool _drawConnections;
        [SerializeField] private Material _lineRenderMaterial;
        [SerializeField, Range(1, 50)] private int _gridWidth;
        [SerializeField, Range(1, 50)] private int _gridDepth;
        HexNode HexNodePrefab;

        private Unit _unitPrefab;
        private Action TileInitCB;
        public Dictionary<Vector2, HexNode> Tiles { get; private set; }

        private void Awake()
        {
            _unitPrefab = LoadTool.LoadPrefab("Unit").GetComponent<Unit>();
            HexNodePrefab = LoadTool.LoadTile("DefaultHex").GetComponent<HexNode>();

            if(_lineRenderMaterial == null)
            {
                _lineRenderMaterial = new Material(Shader.Find("Unlit/Color"));
                _lineRenderMaterial.color = Color.black;
            }
        }

        private void Start() {
            Tiles = GenerateGrid();

            foreach (var tile in Tiles.Values)
            {
                tile.CacheNeighbors();
            }

            SpawnUnits();
            HexNode.OnHoverTile += OnTileHover;

            if (TileInitCB != null)
            {
                TileInitCB.Invoke();
            }
        }

        private void OnDestroy() => HexNode.OnHoverTile -= OnTileHover;


        private HexNode LoadHexByName(string hexName)
        {
            return LoadTool.LoadTile(hexName).GetComponent<HexNode>();
        }

        public Dictionary<Vector2, HexNode> GenerateGrid()
        {
            var tiles = new Dictionary<Vector2, HexNode>();
            var grid = new GameObject
            {
                name = "Grid"
            };
            for (var r = 0; r < _gridDepth; r++)
            {
                var rOffset = r >> 1;
                for (var q = -rOffset; q < _gridWidth - rOffset; q++)
                {
                    HexNode tile;
                    // 准备通过配置加载地面
                    string name = TempIdToPrefabName.FindName(new Vector2Int(q, r));
                    if (name != null)
                    {
                        tile = Instantiate(LoadHexByName(name), grid.transform);
                    }
                    else
                    {
                        tile = Instantiate(LoadHexByName("ObstacleHex"), grid.transform);
                    }



                    tile.Init(CalcuGridType(q, r), new HexCoords(q, r));
                    tiles.Add(tile.Coords.MapCoord, tile);
                    tile.SetLineMaterial(_lineRenderMaterial);
                    tile.UpdateLineRenderer();
                }
            }
            return tiles;
        }

        public void ShowAllBorders()
        {
            foreach(var hex in Tiles.Values)
            {
                hex.ShowBorder();
            }
        }

        public void HideAllBorders()
        {
            foreach (var hex in Tiles.Values)
            {
                hex.HideBorder();
            }
        }

        public void SetTileInitCB(Action action)
        {
            TileInitCB = action;
        }

        public HexNode GetTileByCoord(Vector2 pos) => Tiles.TryGetValue(pos, out var tile) ? tile : null;

        private GridType CalcuGridType(int q, int r)
        {
            return GridType.Road;
        }
    }
}