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
            isRotating = true;
            agent.speed = 0f;
        }

    }

    //_____________________旋转功能部分，当开启时，单位必须先旋转，然后再移动。（大型单位旋转方法）——————————————
    public bool enableRotateBeforeMove = false;
    protected bool isRotating = false; // 是否正在旋转
    [SerializeField]
    protected float rotationThreshold = 10f; // 旋转阈值，小于该值认为旋转完成
    //请注意，以下两个Reference在RotateTarget之中更新，如果没有执行RotateTarget方法更新就提取，将会得到错误的内容。
    protected Vector3 targetDirection;
    protected Quaternion targetRotation;

    /// <summary>
    /// In each frame, rotate unit toward target
    /// 在每帧中，都会将单位向着目标位置旋转一点。
    /// </summary>
    /// <param name="targetPosition"></param>
    protected void RotateToTarget() {

        // 获取目标方向向量
        targetDirection = agent.destination - agent.transform.position;

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
        if (isRotating)
        {
            RotateToTarget();
        }
    }

    protected void FixedUpdate()
    {
        bool isFacingTarget = IfTowardTarget();
        if (isFacingTarget && isRotating)
        {
            isRotating = false;
            agent.speed = normalSpeed;
        }
    }




}
