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
        public List<Transform> componentListOnTower;
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
            if (componentListOnTower.Count <= 0)
            {
                return;
            }


            //foreach (Transform compoent in componentListOnTower)
            //{
            //      使用插值方法逐渐将物体旋转到目标方向
            //      compoent.rotation = Quaternion.RotateTowards(compoent.rotation, targetRotation, (agent.angularSpeed * towerTurnValue) * Time.deltaTime);
            //}     

            // 获取目标方向向量
            targetDirection = target - componentListOnTower[0].position;
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

        public override void MoveToWardTargetTrigger(Vector3 _targetPosition) {
            base.MoveToWardTargetTrigger(_targetPosition);
        }

        /// <summary>
        ///  向着目标方向进行攻击的主逻辑
        /// </summary>
        /// <param name="targetLocation">目标的位置</param>
        public override void AttackLogicTrigger()
        {
            LockOnTargetTrigger();
            base.AttackLogicTrigger();
        }

        protected override void GiveUpAttack()
        {
            LockOnTargetOffTrigger();
            base.GiveUpAttack();
        }

        protected void LockOnTargetTrigger()
        {
            lockOnTarget = true;
        }
        protected void LockOnTargetOffTrigger()
        {
            lockOnTarget = false;
        }

        protected void DetectTargetNear() {
            if (enemyUnitTarget != null)
            {
                if (Vector3.Distance(enemyUnitTarget.getPosition(), agent.transform.position) > attackRange)
                {
                    //Debug.Log("移动目标超出距离的攻击距离，停止炮管锁定");
                    LockOnTargetOffTrigger();
                }
                else
                {
                    //Debug.Log(Vector3.Distance(enemyUnitTarget.getPosition(), componentListOnTower[0].position));
                    //Debug.Log("移动目标未超出距离的攻击距离，持续炮管锁定");
                    LockOnTargetTrigger();
                }
            }
        }

        protected override void RotateToTarget(Vector3 targetPosition)
        {
            if (!lockOnTarget) {
                TurnTower(targetPosition);
            }

            base.RotateToTarget(targetPosition);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            DetectTargetNear();

            if (lockOnTarget)
            {
                //锁定目标时，向着目标转动炮台。
                TurnTower(enemyUnitTarget.transform.position);
            }
            else {
                if (!IfTowardTarget(componentListOnTower[0]))
                {
                    TurnTower(agent.destination + agent.transform.forward);
                }

                
            }
        }

    }
}
