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
        protected bool lockOnTarget = false;
        protected Vector3 targetLocation;

        /// <summary>
        /// 旋转炮塔，以及一切被存放在炮塔上的物体。
        /// 请注意，记得要将需要旋转的物体，托选进入上方的componentListOnTower中。
        /// </summary>
        protected void TurnTower(Vector3 target) {
            if (componentListOnTower.Length <= 0)
            {
                return;
            }


            //foreach (Transform compoent in componentListOnTower)
            //{
            //      使用插值方法逐渐将物体旋转到目标方向
            //      compoent.rotation = Quaternion.RotateTowards(compoent.rotation, targetRotation, (agent.angularSpeed * towerTurnValue) * Time.deltaTime);
            //}     

            // 获取目标方向向量
            targetDirection = target - agent.transform.position;
            // 计算目标旋转角度
            targetRotation = Quaternion.LookRotation(targetDirection);

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

        /// <summary>
        ///  向着目标方向进行攻击的主逻辑
        /// </summary>
        /// <param name="targetLocation">目标的位置</param>
        public override void AttackLogic()
        {
            Debug.Log("攻击主逻辑开启。");
            Vector3 _targetLocation = enemyUnitTarget.transform.position;
            if (InAttackRange(_targetLocation))
            {
                Debug.Log("Stop Movement!");
                StopMovement();
                Debug.Log("目标在攻击距离之内，进行攻击！");
                //坦克类的在攻击时，在炮塔上的旋转功能
                LockOnTarget();
                StartFireTrigger();
            }
            else
            {
                Debug.Log("目标在攻击距离之外，移动到目标区域内进行攻击！");
                MoveCloser(_targetLocation);
            }
        }

        private void LockOnTarget()
        {
            Debug.Log("向目标旋转炮管。");
            lockOnTarget = true;
        }

        protected void Update()
        {
            base.Update();

        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAutoRotating && !lockOnTarget)
            {
                //自动寻路时，转向朝向前进的目标位置。
                TurnTower(agent.destination);
            }

            if (isAutoRotating && lockOnTarget)
            {
                //自动寻路时，转向朝向前进的目标位置。
                TurnTower(agent.destination);
            }
        }
    }
}
