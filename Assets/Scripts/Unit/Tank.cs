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

        //——————————————————————————————————————————————————————————————————————炮塔的正确旋转——————————————————————————————————————————————————————————————————
        //用于记录炮塔的旋转角度
        protected Quaternion compoent_TargetRotation;
        //用于限制炮塔的x轴旋转角度（视觉效果是上下）
        protected float maxRotationAngle = 45f;
        /// <summary>
        /// 旋转炮塔，以及一切被存放在炮塔上的物体。
        /// 请注意，记得要将需要旋转的物体，托选进入上方的componentListOnTower中。
        /// </summary>
        protected void TurnTower() {
            if (componentListOnTower.Length <= 0)
            {
                return;
            }


            //foreach (Transform compoent in componentListOnTower)
            //{
            //      使用插值方法逐渐将物体旋转到目标方向
            //      compoent.rotation = Quaternion.RotateTowards(compoent.rotation, targetRotation, (agent.angularSpeed * towerTurnValue) * Time.deltaTime);
            //}     


            foreach (Transform compoent in componentListOnTower)
            {
                // 将当前的旋转角度转换为欧拉角
                Vector3 currentRotation = compoent.rotation.eulerAngles;

                // 限制X轴旋转角度在指定范围内
                float clampedAngle = Mathf.Clamp(currentRotation.x, -maxRotationAngle, maxRotationAngle);

                // 创建限制后的欧拉角
                Vector3 limitedRotation = new Vector3(clampedAngle, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z);

                // 将限制后的欧拉角转换为旋转角度
                Quaternion limitedQuaternion = Quaternion.Euler(limitedRotation);

                //使用插值方法逐渐将物体旋转到目标方向
                compoent.rotation = Quaternion.RotateTowards(compoent.rotation, limitedQuaternion, (agent.angularSpeed * towerTurnValue) * Time.deltaTime);
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
