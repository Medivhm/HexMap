using System;
using System.Collections.Generic;
using UnityEngine;

public enum GridType
{
    Pointy,
    Flat,
}

namespace Tiles {
    // 游戏内容
    public partial class GridManager : MonoSingleton<GridManager>
    {
        public Unit SpawnPlayerUnit(Vector2 pos, GameObject playerPrfab)
        {
            return SpawnPlayerUnit(Tiles[pos], playerPrfab);
        }

        public Unit SpawnPlayerUnit(HexNode node, GameObject playerPrfab)
        {
            Unit unit = GameObject.Instantiate(playerPrfab, node.Coords.WorldPos, Quaternion.identity).GetComponent<Unit>();
            unit.Init();
            unit.HexCoord = node.Coords.MapCoord;
            return unit;
        }

        public Unit SpawnSpriteUnit(Vector2 pos, Sprite sprite)
        {
            return SpawnSpriteUnit(Tiles[pos], sprite);
        }

        public Unit SpawnSpriteUnit(HexNode node, Sprite sprite)
        {
            Unit unit = GameObject.Instantiate(_unitPrefab, node.Coords.WorldPos, Quaternion.Euler(Main.Instance.MainCameraController.EulerRotateX, 0f, 0f));
            unit.Init();
            unit.SetSprite(sprite);
            unit.HexCoord = node.Coords.MapCoord;
            return unit;
        }

        private void OnTileHover(HexNode HexNode)
        {
            //if (Main.Instance.Mode != GameMode.Play) return;
            //if (Main.Instance.MainPlayer.Unit.IsMoving) return; // 移动时不可改移动目标

            foreach (var t in Tiles.Values) t.RevertTile();
            Player player = Main.Instance.MainPlayer;
            CameraController cameraCtrl = Main.Instance.MainCameraController;

            List<HexNode> passNodes = Pathfinding.FindPath(GetTileByCoord(player.Unit.HexCoord), HexNode);

            if (HexNode.Coords.MapCoord.Equals(player.Unit.HexCoord) && Vector3.Distance(player.Unit.transform.position, HexNode.transform.position) > 0.002f)
            {
                // 如果往外走但还在格子里又点击了这个格子
                passNodes.Add(HexNode);
            }
            if (passNodes != null && passNodes.Count > 0 )
            {
                if (player.IsOutOfScreen()) // 如果在屏幕外
                {
                    cameraCtrl.DisableTouch();  // 禁止触摸移动放大地图
                    player.Unit.StopMove();
                    cameraCtrl.MoveToPlayer(() =>
                    {
                        // 摄像机移动，使得角色出现在左屏中央后
                        cameraCtrl.FollowPlayer();
                        cameraCtrl.EnableTouch();  // 恢复触摸
                        player.Unit.MoveTo(GetTileByCoord(player.Unit.HexCoord), HexNode, passNodes,  // 角色开始移动
                            (_, _, _) =>
                            {
                                cameraCtrl.CameraMoveToHex(HexNode);
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
        [SerializeField, Range(1, 50)] private int _gridHeight;
        [SerializeField] private GridType _gridType;
        HexNode HexNodePrefab;

        private Unit _unitPrefab;
        private Action TileInitCB;
        public Dictionary<Vector2, HexNode> Tiles { get; private set; }
        public GridType GridType => _gridType;
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

            if (GridType.Pointy == _gridType)
            {

                for (var r = 0; r < _gridHeight; r++)
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

                        tile.Init(CalcuHexType(q, r), new HexCoords(q, r));
                        tiles.Add(tile.Coords.MapCoord, tile);
                        tile.SetLineMaterial(_lineRenderMaterial);
                        tile.UpdateLineRenderer();
                    }
                }
            }
            else if (GridType.Flat == _gridType)
            {
                for (int q = 0; q <  _gridWidth; q++)
                {
                    int qOffset = q >> 1;
                    for (int r = -qOffset; r < _gridHeight - qOffset; r++)
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

                        tile.Init(CalcuHexType(q, r), new HexCoords(q, r));
                        tile.RotateIconToFlat();
                        tiles.Add(tile.Coords.MapCoord, tile);
                        tile.SetLineMaterial(_lineRenderMaterial);
                        tile.UpdateLineRenderer();
                    }
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

        private HexType CalcuHexType(int q, int r)
        {
            return HexType.Road;
        }
    }
}