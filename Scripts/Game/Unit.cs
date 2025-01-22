using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tiles
{
    public class Unit : MonoBehaviour {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Animator _animator;
        private float moveSpeed = 5f;
        public Vector2 HexCoord;
        public bool IsMoving => _isMoving;

        public void Init(Sprite sprite) {
            _renderer.sprite = sprite;
        }

        public void SetAnimatorEnabled(bool state)
        {
            _animator.enabled = state;
        }

        private bool _isMoving;
        private HexNode _fromNode;
        private HexNode _targetNode;
        private List<HexNode> _passNodes;
        private int _passCount;
        private Action<Unit, HexNode, HexNode> _moveFallback;
        public void MoveTo(HexNode fromNode, HexNode targetNode, List<HexNode> passNodes, Action<Unit, HexNode, HexNode> moveFallback = null)
        {
            // 如果半途换路，看看是否在中间，如果在中间就不走回去
            float sighXPre = Mathf.Sign(this.transform.position.x - fromNode.Coords.WorldPos.x);
            float sighXNext = Mathf.Sign(passNodes[passNodes.Count - 1].Coords.WorldPos.x - this.transform.position.x);
            float sighYPre = Mathf.Sign(this.transform.position.y - fromNode.Coords.WorldPos.y);
            float sighYNext = Mathf.Sign(passNodes[passNodes.Count - 1].Coords.WorldPos.y - this.transform.position.y);
            if (!(sighXPre * sighXNext > 0 && sighYPre * sighYNext > 0))
            {
                passNodes.Add(fromNode);
            }

            _passCount = passNodes.Count - 1;
            _isMoving = true;
            _fromNode = fromNode;
            _targetNode = targetNode;
            _passNodes = passNodes;
            _moveFallback = moveFallback;
        }

        private void MoveToNode(HexNode to)
        {
            if(Move(to.Coords.WorldPos))
            {
                _passCount--;
            }
        }

        Vector3 dir;
        private bool Move(Vector3 targetPos)
        {
            dir = (targetPos - this.transform.position).normalized;
            this.transform.position += dir * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(targetPos, this.transform.position) < 0.03f)
            {
                HexCoord = _passNodes[_passCount].Coords.MapCoord;

                if (_passCount == 0)
                {
                    if (_moveFallback != null)
                    {
                        _moveFallback.Invoke(this, _fromNode, _targetNode);
                        _moveFallback = null;
                    }
                    _passNodes = null;
                    _isMoving = false;
                }

                this.transform.position = targetPos;
                return true;
            }
            return false;
        }

        private void Update()
        {
            if (_isMoving)
            {
                MoveToNode(_passNodes[_passCount]);
            }
        }
    }
}
