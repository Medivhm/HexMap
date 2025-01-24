using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tiles
{
    public class Unit3D : Unit
    {
        public Vector3 FaceTo
        {
            get
            {
                float theta = Mathf.Deg2Rad * (transform.rotation.eulerAngles.z + 90f);
                return -new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));
            }
        }

        public override void Init()
        {
            base.Init();
            this.transform.GetChild(0).transform.rotation = Quaternion.Euler(90f, -90f, 90f);
            this.RotateTo(MapDirection.Point_E);
        }

        protected override void MoveToNode(HexNode to)
        {
            if (Move(to.Coords.WorldPos, (to.Coords.WorldPos - this.transform.position).normalized))
            //if (Move(to.Coords.WorldPos, FaceTo))  // ���ƶ�������ͬ����
            {
                passCount--;
                if (isMoving) // �����û�ߵ��յ�վ
                {
                    isMoving = false;
                    RotateTo(passNodes[passCount], () =>
                        {
                            isMoving = true;
                        });
                }
            }
        }

        protected override void Update()
        {
            if (_isRotating)
            {
                SetAnimBool("turn", true);
                Rotate();
            }
            if (isMoving)
            {
                SetAnimBool("walk", true);
                MoveToNode(passNodes[passCount]);
            }

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Alpha1))
            {
                PlayAnim("Dance");
            }
#endif
        }
    }
}