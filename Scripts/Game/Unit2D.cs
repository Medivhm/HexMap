using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tiles
{
    public class Unit2D : Unit
    {

        public override void Init()
        {
            base.Init();
            this.transform.GetChild(0).rotation = Quaternion.Euler(Main.Instance.MainCameraController.transform.eulerAngles.x, 0f, 0f);
        }

        protected override void MoveToNode(HexNode to)
        {
            if (Move(to.Coords.WorldPos, (to.Coords.WorldPos - this.transform.position).normalized))
            {
                passCount--;
            }
        }

        protected override void Update()
        {
            if (isMoving)
            {
                PlayAnim("Walk");
                MoveToNode(passNodes[passCount]);
            }
        }
    }
}