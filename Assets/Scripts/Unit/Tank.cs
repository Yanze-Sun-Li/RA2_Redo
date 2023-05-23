using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unit
{
    class Tank : UnitControl
    {
        //在坦克主体上的炮塔
        public Transform[] componentListOnTower;
        public float towerTurnValue = 1.4f;

        /// <summary>
        /// 旋转炮塔，以及一切被存放在炮塔上的物体。
        /// 请注意，记得要将需要旋转的物体，托选进入上方的componentListOnTower中。
        /// </summary>
        protected void TurnTower() {
            if (componentListOnTower.Length <= 0)
            {
                return;
            }


            foreach (Transform compoent in componentListOnTower)
            {

                // 使用插值方法逐渐将物体旋转到目标方向
                compoent.rotation = Quaternion.RotateTowards(compoent.rotation, targetRotation, (agent.angularSpeed * towerTurnValue) * Time.deltaTime);
                
               
            }            
        }


        protected void Update()
        {
            base.Update();
            if (isRotating)
            {
                TurnTower();
            }
        }
    }
}
