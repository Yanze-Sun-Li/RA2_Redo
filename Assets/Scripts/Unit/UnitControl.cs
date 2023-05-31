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
            isAutoRotating = true;
            agent.speed = 0f;
        }

    }

    //_____________________��ת���ܲ��֣�������ʱ����λ��������ת��Ȼ�����ƶ��������͵�λ��ת����������������������������������
    public bool enableRotateBeforeMove = false;
    protected bool isAutoRotating = false; // �Ƿ������Զ���ת��Ŀ�귽λ
    [SerializeField]
    protected float rotationThreshold = 10f; // ��ת��ֵ��С�ڸ�ֵ��Ϊ��ת���
    //��ע�⣬��������Reference��RotateTarget֮�и��£����û��ִ��RotateTarget�������¾���ȡ������õ���������ݡ�
    protected Vector3 targetDirection;
    protected Quaternion targetRotation;

    /// <summary>
    /// In each frame, rotate unit toward target
    /// ��ÿ֡�У����Ὣ��λ����Ŀ��λ����תһ�㡣
    /// </summary>
    /// <param name="targetPosition">��ת���Ŀ��λ��</param>
    protected void RotateToTarget(Vector3 targetPosition) {

        // ��ȡĿ�귽������
        targetDirection = targetPosition - agent.transform.position;

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

    //��������������������������������������������������������������������Ϊ������������������������������������������������������������������������������������������������������������������������������������������������
    //Attack Variables ������ر���
    //��������
    [SerializeField]
    protected float attackRange = 10f;
    [SerializeField]
    protected float attackDamage = 5f;
    [SerializeField]
    protected bool attackEnemy = false;
    //Enemy Located
    //���ֵо�
    protected bool enemyLocated = false;
    //Ŀǰ��֧�ֶԵз��ƶ���λ��ɹ�����
    public UnitControl enemyUnitTarget;
    //--�����ƣ� ��ǰʱ�����޶Եط�������λ���й�����reference��

    //������ȴʱ��
    [SerializeField]
    protected float attackCD = 0.3f;
    protected bool readyToAttack = true;

    /// <summary>
    /// ����һ��������Ŀ�ꡣ
    /// </summary>
    /// <param name="enemy">�������й�����Ŀ��</param>
    public void SetEnemy(UnitControl enemy) {
        enemyUnitTarget = enemy;
    }

    /// <summary>
    ///  ����Ŀ�귽����й��������߼���
    ///  ��ʹ�ø÷���֮ǰ������ʹ��SetEnemy()��enemyUnitTarget����Ŀ�꣡
    /// </summary>
    /// <param name="targetLocation">Ŀ���λ��</param>
    public virtual void AttackLogic() {
        Vector3 targetLocation = enemyUnitTarget.transform.position;
        if (InAttackRange(targetLocation))
        {
            //̹������ڹ���ʱ���������ϵ���ת����
            //LockOnTarget(targetLocation);
            StartFireTrigger();
        }
        else
        {
            MoveCloser(targetLocation);
        }
    }


    /// <summary>
    /// Ŀ�����ڵ��Ƿ��ڵ�λ�Ĺ�����Χ֮�ڣ�
    /// </summary>
    /// <param name="targetLocation">Ŀ��ķ����ǣ�</param>
    /// <returns>�Ƿ��ڹ�����Χ֮�ڣ�</returns>
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
    /// ��ǰ��λ���빥��״̬
    /// </summary>
    /// <param name="enemy">������ָ������</param>
    protected void StartFireTrigger() {
        enemyLocated = true;
        attackEnemy = true;
        
    }

    /// <summary>
    /// �Եط���λ����˺���
    /// </summary>
    protected void Attack() {
        enemyUnitTarget.UnderAttack(attackDamage);
    }

    IEnumerator AttackCDCounter() {
        yield return new WaitForSeconds(attackCD);
        readyToAttack = true;
    }

    //�ƶ���������Χ�������
    protected void MoveCloser(Vector3 targetLocation)
    {
        // ��ȡĿ������������ߵ�
        Vector3 nearestAttackPosition = GetNearestAttackPosition(targetLocation);

        // �ƶ�������Ŀ����ߵ�
        agent.SetDestination(nearestAttackPosition);
    }

    /// <summary>
    /// Ѱ�ҵ�ǰ��Ŀ��ص���Χ���ʺϿ���ĵص�
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

        // ���û���ҵ������ߵ㣬�򷵻�Ŀ��λ��
        return targetPosition;
    }


    //��������������������������������������������������������������λ�ܵ���������������������������������������������������������������������������������������������������������������������������������������������������������������������

    /// <summary>
    /// ��λ��������...
    /// </summary>
    /// <param name="attackDamage">���������˺���ֵ</param>
    protected void UnderAttack(float attackDamage) {
        Debug.Log("��λ" + gameObject.name + "������һ��" + attackDamage + "���˺��Ĺ�����"); 
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
