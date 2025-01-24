using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UIElements;

namespace Tiles
{
    public static class MapDirection
    {
        private static readonly float Sqrt3 = Mathf.Sqrt(3);

        public static readonly Quaternion Point_NE = Quaternion.Euler(0f, 0f, 145f);
        public static readonly Quaternion Point_E  = Quaternion.Euler(0f, 0f, 90f);
        public static readonly Quaternion Point_SE = Quaternion.Euler(0f, 0f, 45f);
        public static readonly Quaternion Point_SW = Quaternion.Euler(0f, 0f, -45f);
        public static readonly Quaternion Point_W  = Quaternion.Euler(0f, 0f, -90f);
        public static readonly Quaternion Point_NW = Quaternion.Euler(0f, 0f, -135f);

        public static readonly Quaternion Flat_NE = Quaternion.Euler(0f, 0f, 120f);
        public static readonly Quaternion Flat_SE = Quaternion.Euler(0f, 0f, 60f);
        public static readonly Quaternion Flat_S  = Quaternion.Euler(0f, 0f, 0f);
        public static readonly Quaternion Flat_SW = Quaternion.Euler(0f, 0f, -60f);
        public static readonly Quaternion Flat_NW = Quaternion.Euler(0f, 0f, -120f);
        public static readonly Quaternion Flat_N  = Quaternion.Euler(0f, 0f, -180f);

        public static Quaternion SideToMapDirection(SidePos sidePos)
        {
            if(GridManager.Instance.GridType == GridType.Pointy)
            {
                switch (sidePos)
                {
                    case SidePos.NE:
                        return MapDirection.Point_NE;
                    case SidePos.E:
                        return MapDirection.Point_E;
                    case SidePos.SE:
                        return MapDirection.Point_SE;
                    case SidePos.SW:
                        return MapDirection.Point_SW;
                    case SidePos.W:
                        return MapDirection.Point_W;
                    case SidePos.NW:
                        return MapDirection.Point_NW;
                }
            }
            else
            {
                switch (sidePos)
                {
                    case SidePos.NE:
                        return MapDirection.Flat_N;
                    case SidePos.E:
                        return MapDirection.Flat_NE;
                    case SidePos.SE:
                        return MapDirection.Flat_SE;
                    case SidePos.SW:
                        return MapDirection.Flat_S;
                    case SidePos.W:
                        return MapDirection.Flat_SW;
                    case SidePos.NW:
                        return MapDirection.Flat_NW;
                }
            }

            return Quaternion.identity;
        }

        public static Quaternion PosFaceToNodeMapDirection(Vector3 pos, HexNode node)
        {
            Vector3 nodePos = node.Coords.WorldPos;
            Vector3 diff = pos - nodePos;
            if (GridManager.Instance.GridType == GridType.Pointy)
            {
                if (diff.x > 0 && Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
                {
                    if (diff.y > 0)
                    {
                        return MapDirection.Point_SW;
                    }
                    else
                    {
                        return MapDirection.Point_NW;
                    }
                }
                else if (diff.x < 0 && Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
                {
                    if (diff.y > 0)
                    {
                        return MapDirection.Point_SE;
                    }
                    else
                    {
                        return MapDirection.Point_NE;
                    }
                }
                else if (diff.x > 0)
                {
                    return MapDirection.Point_W;
                }
                else
                {
                    return MapDirection.Point_E;
                }
            }
            else
            {
                if (diff.x > 0 && Mathf.Abs(diff.y) < Sqrt3 * diff.x)
                {
                    if (diff.y > 0)
                    {
                        return MapDirection.Flat_SW;
                    }
                    else
                    {
                        return MapDirection.Flat_NW;
                    }
                }
                else if (diff.x < 0 && Mathf.Abs(diff.y) < Sqrt3 * Mathf.Abs(diff.x))
                {
                    if (diff.y > 0)
                    {
                        return MapDirection.Flat_SE;
                    }
                    else
                    {
                        return MapDirection.Flat_NE;
                    }
                }
                else if (diff.y > 0)
                {
                    return MapDirection.Flat_S;
                }
                else
                {
                    return MapDirection.Flat_N;
                }
            }
        }
    }

    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1.8f;
        [SerializeField] private float _rotateDegPerSecond = 3f;
        private SpriteRenderer _renderer;
        private Animator _animator;

        protected QuickTimer quickTimer;

        public Vector2Int HexCoord;
        public bool IsMoving => isMoving;

        public virtual void Init()
        {
            quickTimer = new QuickTimer();
            _animator = GetComponentInChildren<Animator>();
        }

        public virtual void SetSprite(Sprite sprite)
        {
            if(_renderer != null)
            {
                _renderer.sprite = sprite;
            }
        }

        protected bool isMoving = false;
        protected List<HexNode> passNodes;
        protected int passCount;
        private HexNode _fromNode;
        private HexNode _targetNode;
        private Action<Unit, HexNode, HexNode> _moveCB;
        public void MoveTo(HexNode fromNode, HexNode targetNode, List<HexNode> ppassNodes, Action<Unit, HexNode, HexNode> moveCB = null)
        {
            // 如果半途换路，看看是否在中间，如果在中间就不走回去
            float sighXPre = Mathf.Sign(this.transform.position.x - fromNode.Coords.WorldPos.x);
            float sighXNext = Mathf.Sign(ppassNodes[ppassNodes.Count - 1].Coords.WorldPos.x - this.transform.position.x);
            float sighYPre = Mathf.Sign(this.transform.position.y - fromNode.Coords.WorldPos.y);
            float sighYNext = Mathf.Sign(ppassNodes[ppassNodes.Count - 1].Coords.WorldPos.y - this.transform.position.y);
            if (!(sighXPre * sighXNext > 0 && sighYPre * sighYNext > 0))
            {
                ppassNodes.Add(fromNode);
            }
            if (this is Unit3D)
            {
                isMoving = false;
                RotateTo(ppassNodes[ppassNodes.Count - 1], () =>
                {
                    isMoving = true;
                });
            }
            else
            {
                isMoving = true;
            }
            passCount = ppassNodes.Count - 1;
            _fromNode = fromNode;
            _targetNode = targetNode;
            this.passNodes = ppassNodes;
            _moveCB = moveCB;
        }

        protected abstract void MoveToNode(HexNode to);

        protected bool Move(Vector3 targetPos, Vector3 dir)
        {
            this.transform.position += dir * _moveSpeed * Time.deltaTime;

            if (Vector3.Distance(targetPos, this.transform.position) < 0.03f)
            {
                HexCoord = passNodes[passCount].Coords.MapCoord;

                if (passCount == 0)
                {
                    if (_moveCB != null)
                    {
                        _moveCB.Invoke(this, _fromNode, _targetNode);
                        _moveCB = null;
                    }
                    passNodes = null;
                    isMoving = false;
                    SetAnimBool("walk", false);
                }

                this.transform.position = targetPos;
                return true;
            }
            return false;
        }

        protected bool _isRotating = false;
        Quaternion _targetRotation;
        Action _rotateCB;
        public void RotateTo(Vector3 targetDir, Action rotateCB = null)
        {
            RotateTo(Quaternion.Euler(targetDir), rotateCB);
        }

        public virtual void RotateTo(HexNode node, Action rotateCB = null)
        {
            if (Vector3.Distance(this.transform.position, node.Coords.WorldPos) < 0.01f)
            {
                Debug.Log(Vector3.Distance(this.transform.position, node.Coords.WorldPos).ToString());
                if (rotateCB != null)
                {
                    rotateCB.Invoke();
                    return;
                }
            }

            SidePos sidePos = GridManager.Instance.GetTileByCoord(this.HexCoord).OtherInWhere(node);
            Quaternion quaternion;
            if(SidePos.None == sidePos)
            {
                quaternion = MapDirection.PosFaceToNodeMapDirection(this.transform.position, node);
            }
            else
            {
                quaternion = MapDirection.SideToMapDirection(sidePos);
            }

            RotateTo(quaternion, rotateCB);
        }

        public void RotateTo(Quaternion targetDir, Action rotateCB = null)
        {
            _isRotating = true;
            _rotateCB = rotateCB;
            _targetRotation = targetDir;
        }

        float threshold = 0.3f;   // 旋转完成范围
        //float coeff;
        protected void Rotate()
        {
            //coeff = rotationSpeed * Time.deltaTime;
            //transform.rotation = Quaternion.Slerp(
            //    transform.rotation,
            //    _targetRotation,
            //    coeff > 0.03f ? coeff : 0.03f
            //);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotateDegPerSecond);
            if(Quaternion.Angle(transform.rotation, _targetRotation) < threshold)
            {
                transform.rotation = _targetRotation;

                _isRotating = false;
                if(_rotateCB != null)
                {
                    _rotateCB.Invoke();
                    _rotateCB = null;
                }
                SetAnimBool("turn", false);
            }
        }

        string nowAnimPlay;
        public void PlayAnim(string animName, float fadeTime = 0f, Action callback = null, float normalizedTimeOffset = 0, float normalizedTransitionTime = 1)
        {
            if (animName == nowAnimPlay) return;
            if (_animator == null) return;

            nowAnimPlay = animName;
            _animator.CrossFade(animName, fadeTime, -1, normalizedTimeOffset, normalizedTransitionTime);
            if (callback != null)
            {
                float time = Util.GetAnimationClip(_animator, animName).length;
                quickTimer.AddTimer(time, callback);
            }
        }

        public void SetAnimBool(string name, bool state)
        {
            if (_animator == null) return;

            _animator.SetBool(name, state);
            Debug.LogFormat("{0}_{1}", name, state.ToString());
        }


        protected abstract void Update();

        protected void OnDestroy()
        {
            quickTimer.DestroyTimers();
        }
    }
}
