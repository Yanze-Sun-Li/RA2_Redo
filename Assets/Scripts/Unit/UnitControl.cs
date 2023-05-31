using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitControl : MonoBehaviour
{
    //—————————————————————————信息类————————————————————————————————
    protected NavMeshAgent agent;

    //—————————————————————————属性类————————————————————————————————
    public int id;
    public int playerID;
    public string name;
    public string tag;
    public float normalSpeed;
    

    //——————————————————————用于向其他的Component提供信息的功能————————————————————————————————————
    /// <summary>
    /// 给予访问者当前单位的位置在什么地方。
    /// </summary>
    /// <returns>以Vector3返回的xyz位置信息。</returns>
    public Vector3 getPosition() {
        return gameObject.transform.position;
    }

    //—————————————————————功能部分，单位能够进行的行为————————————————————————————————————————
    //目标位置记录：
    Vector3 targetPosition;

    /// <summary>
    /// 对向着目标方向前进的方式进行判断
    /// </summary>
    /// <param name="targetPosition">目标位置</param>
    public void MoveToWardTargetTrigger(Vector3 _targetPosition) {
        targetPosition = _targetPosition;
        agent.SetDestination(targetPosition);
        //如果没有开启必须旋转后移动：
        if (!enableRotateBeforeMove)
        {
            Debug.Log("trying to moving!" + " Target: " + targetPosition);
           
        }else
        //如果开启了必须旋转后移动，触发触发器。
        {
            Debug.Log("trying to rotate before moving!" + " Target: " + targetPosition);
            isAutoRotating = true;
            agent.speed = 0f;
        }

    }

    //_____________________旋转功能部分，当开启时，单位必须先旋转，然后再移动。（大型单位旋转方法）——————————————
    public bool enableRotateBeforeMove = false;
    protected bool isAutoRotating = false; // 是否正在自动旋转至目标方位
    [SerializeField]
    protected float rotationThreshold = 10f; // 旋转阈值，小于该值认为旋转完成
    //请注意，以下两个Reference在RotateTarget之中更新，如果没有执行RotateTarget方法更新就提取，将会得到错误的内容。
    protected Vector3 targetDirection;
    protected Quaternion targetRotation;

    /// <summary>
    /// In each frame, rotate unit toward target
    /// 在每帧中，都会将单位向着目标位置旋转一点。
    /// </summary>
    /// <param name="targetPosition">旋转向的目标位置</param>
    protected void RotateToTarget(Vector3 targetPosition) {

        // 获取目标方向向量
        targetDirection = targetPosition - agent.transform.position;

        // 计算目标旋转角度
        targetRotation = Quaternion.LookRotation(targetDirection);

        // 创建限制后的欧拉角
        Vector3 limitedRotation = new Vector3(0, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z);

        // 将限制后的欧拉角转换为旋转角度
        Quaternion limitedQuaternion = Quaternion.Euler(limitedRotation);

        // 使用插值方法逐渐将物体旋转到目标方向
        agent.transform.rotation = Quaternion.RotateTowards(transform.rotation, limitedQuaternion, agent.angularSpeed * Time.deltaTime);

    }

    /// <summary>
    /// 当前的Agent物体是否正面向目标位置？
    /// </summary>
    /// <returns></returns>
    protected bool IfTowardTarget() {
        // 获取目标方向向量
        targetDirection = agent.destination - agent.transform.position;

        // 获取当前朝向向量
        Vector3 currentDirection = agent.transform.forward;

        // 计算角度差
        float angleDifference = Vector3.Angle(targetDirection, currentDirection);

        // 判断角度差是否小于阈值
        return angleDifference < rotationThreshold;
    }


    //—————————————————————初始化，默认设置刷新等相关功能———————————————————————————————————————
    /// <summary>
    /// 将单位信息刷新为初始状态。
    /// </summary>
    public void InfoInitial() 
    {
        id = -1;
        playerID = -1;
        name = null;
        tag = null;

    }

    //———————————————————————————————攻击行为————————————————————————————————————————————————————————————————————————
    //Attack Variables 攻击相关变量
    //攻击距离
    [SerializeField]
    protected float attackRange = 10f;
    [SerializeField]
    protected float attackDamage = 5f;
    [SerializeField]
    protected bool attackEnemy = false;
    //Enemy Located
    //发现敌军
    protected bool enemyLocated = false;
    //目前仅支持对敌方移动单位造成攻击。
    public UnitControl enemyUnitTarget;
    //--待完善， 当前时间内无对地方建筑单位进行攻击的reference。

    //攻击冷却时间
    [SerializeField]
    protected float attackCD = 0.3f;
    protected bool readyToAttack = true;

    /// <summary>
    /// 设置一个攻击的目标。
    /// </summary>
    /// <param name="enemy">即将进行攻击的目标</param>
    public void SetEnemy(UnitControl enemy) {
        enemyUnitTarget = enemy;
    }

    /// <summary>
    ///  向着目标方向进行攻击的主逻辑。
    ///  在使用该方法之前，请先使用SetEnemy()或enemyUnitTarget设置目标！
    /// </summary>
    /// <param name="targetLocation">目标的位置</param>
    public virtual void AttackLogic() {
        Vector3 targetLocation = enemyUnitTarget.transform.position;
        if (InAttackRange(targetLocation))
        {
            //坦克类的在攻击时，在炮塔上的旋转功能
            //LockOnTarget(targetLocation);
            StartFireTrigger();
        }
        else
        {
            MoveCloser(targetLocation);
        }
    }


    /// <summary>
    /// 目标所在地是否在单位的攻击范围之内？
    /// </summary>
    /// <param name="targetLocation">目标的方向是？</param>
    /// <returns>是否在攻击范围之内？</returns>
    protected bool InAttackRange(Vector3 targetLocation) {

        if (Vector3.Distance(agent.transform.position, targetLocation) < attackRange)
        {
            return true;
        }
        else {
            return false;
        }
    }

    //protected bool LockOnTarget(Vector3 targetLocation) {


    //}

    /// <summary>
    /// 当前单位进入攻击状态
    /// </summary>
    /// <param name="enemy">攻击的指定敌人</param>
    protected void StartFireTrigger() {
        enemyLocated = true;
        attackEnemy = true;
        
    }

    /// <summary>
    /// 对地方单位造成伤害。
    /// </summary>
    protected void Attack() {
        enemyUnitTarget.UnderAttack(attackDamage);
    }

    IEnumerator AttackCDCounter() {
        yield return new WaitForSeconds(attackCD);
        readyToAttack = true;
    }

    //移动到攻击范围内最近点
    protected void MoveCloser(Vector3 targetLocation)
    {
        // 获取目标点的最近可行走点
        Vector3 nearestAttackPosition = GetNearestAttackPosition(targetLocation);

        // 移动到最近的可行走点
        agent.SetDestination(nearestAttackPosition);
    }

    /// <summary>
    /// 寻找到前往目标地点周围，适合开火的地点
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    protected Vector3 GetNearestAttackPosition(Vector3 targetPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, attackRange, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // 如果没有找到可行走点，则返回目标位置
        return targetPosition;
    }


    //——————————————————————————————单位受到攻击————————————————————————————————————————————————————————————————————————————————

    /// <summary>
    /// 单位被攻击后...
    /// </summary>
    /// <param name="attackDamage">被攻击的伤害数值</param>
    protected void UnderAttack(float attackDamage) {
        Debug.Log("单位" + gameObject.name + "遭受了一次" + attackDamage + "点伤害的攻击。"); 
    }

    //——————————————————————————————Unity游戏引擎默认功能—————————————————————————————————————————————————————————————

    protected void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    protected void Start()
    {
        normalSpeed = agent.speed;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (attackEnemy)
        {
            if (readyToAttack)
            {
                readyToAttack = false;
                StartCoroutine("AttackCDCounter");
                Attack();
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (enemyLocated)
        {
            RotateToTarget(enemyUnitTarget.transform.position);
        }
        else if (isAutoRotating)
        {
            RotateToTarget(agent.destination);
        }
        bool isFacingTarget = IfTowardTarget();
        if (isFacingTarget && isAutoRotating)
        {
            isAutoRotating = false;
            agent.speed = normalSpeed;
        }

    }




}
