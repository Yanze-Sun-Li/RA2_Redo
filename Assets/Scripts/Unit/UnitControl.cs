using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitControl : MonoBehaviour
{
    //����������������������������������������������������Ϣ�ࡪ��������������������������������������������������������������
    protected NavMeshAgent agent;

    //�������������������������������������������������������ࡪ��������������������������������������������������������������
    public int id;
    public int playerID;
    public string name;
    public string tag;
    public float normalSpeed;
    

    //��������������������������������������������������������Component�ṩ��Ϣ�Ĺ��ܡ�����������������������������������������������������������������������
    /// <summary>
    /// ��������ߵ�ǰ��λ��λ����ʲô�ط���
    /// </summary>
    /// <returns>��Vector3���ص�xyzλ����Ϣ��</returns>
    public Vector3 getPosition() {
        return gameObject.transform.position;
    }

    //���������������������������������������������ܲ��֣���λ�ܹ����е���Ϊ��������������������������������������������������������������������������������
    //Ŀ��λ�ü�¼��
    Vector3 targetPosition;

    /// <summary>
    /// ������Ŀ�귽��ǰ���ķ�ʽ�����ж�
    /// </summary>
    /// <param name="targetPosition">Ŀ��λ��</param>
    public void MoveToWardTargetTrigger(Vector3 _targetPosition) {
        targetPosition = _targetPosition;
        agent.SetDestination(targetPosition);
        //���û�п���������ת���ƶ���
        if (!enableRotateBeforeMove)
        {
            Debug.Log("trying to moving!" + " Target: " + targetPosition);
           
        }else
        //��������˱�����ת���ƶ���������������
        {
            Debug.Log("trying to rotate before moving!" + " Target: " + targetPosition);
            isRotating = true;
            agent.speed = 0f;
        }

    }

    //_____________________��ת���ܲ��֣�������ʱ����λ��������ת��Ȼ�����ƶ��������͵�λ��ת����������������������������������
    public bool enableRotateBeforeMove = false;
    protected bool isRotating = false; // �Ƿ�������ת
    [SerializeField]
    protected float rotationThreshold = 10f; // ��ת��ֵ��С�ڸ�ֵ��Ϊ��ת���
    //��ע�⣬��������Reference��RotateTarget֮�и��£����û��ִ��RotateTarget�������¾���ȡ������õ���������ݡ�
    protected Vector3 targetDirection;
    protected Quaternion targetRotation;

    /// <summary>
    /// In each frame, rotate unit toward target
    /// ��ÿ֡�У����Ὣ��λ����Ŀ��λ����תһ�㡣
    /// </summary>
    /// <param name="targetPosition"></param>
    protected void RotateToTarget() {

        // ��ȡĿ�귽������
        targetDirection = agent.destination - agent.transform.position;

        // ����Ŀ����ת�Ƕ�
        targetRotation = Quaternion.LookRotation(targetDirection);

        // �������ƺ��ŷ����
        Vector3 limitedRotation = new Vector3(0, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z);

        // �����ƺ��ŷ����ת��Ϊ��ת�Ƕ�
        Quaternion limitedQuaternion = Quaternion.Euler(limitedRotation);

        // ʹ�ò�ֵ�����𽥽�������ת��Ŀ�귽��
        agent.transform.rotation = Quaternion.RotateTowards(transform.rotation, limitedQuaternion, agent.angularSpeed * Time.deltaTime);

    }

    /// <summary>
    /// ��ǰ��Agent�����Ƿ�������Ŀ��λ�ã�
    /// </summary>
    /// <returns></returns>
    protected bool IfTowardTarget() {
        // ��ȡĿ�귽������
        targetDirection = agent.destination - agent.transform.position;

        // ��ȡ��ǰ��������
        Vector3 currentDirection = agent.transform.forward;

        // ����ǶȲ�
        float angleDifference = Vector3.Angle(targetDirection, currentDirection);

        // �жϽǶȲ��Ƿ�С����ֵ
        return angleDifference < rotationThreshold;
    }


    //��������������������������������������������ʼ����Ĭ������ˢ�µ���ع��ܡ�����������������������������������������������������������������������������
    /// <summary>
    /// ����λ��Ϣˢ��Ϊ��ʼ״̬��
    /// </summary>
    public void InfoInitial() 
    {
        id = -1;
        playerID = -1;
        name = null;
        tag = null;

    }

    //������������������������������������������������������������Unity��Ϸ����Ĭ�Ϲ��ܡ�������������������������������������������������������������������������������������������������������������������������

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
