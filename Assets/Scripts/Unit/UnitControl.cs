using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitControl : MonoBehaviour
{
    //����������������������������������������������������Ϣ�ࡪ��������������������������������������������������������������
    private NavMeshAgent agent;
    //�������������������������������������������������������ࡪ��������������������������������������������������������������
    public int id;
    public int playerID;
    public string name;
    public string tag;

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //��������������������������������������������������������Component�ṩ��Ϣ�Ĺ��ܡ�����������������������������������������������������������������������
    /// <summary>
    /// ��������ߵ�ǰ��λ��λ����ʲô�ط���
    /// </summary>
    /// <returns>��Vector3���ص�xyzλ����Ϣ��</returns>
    public Vector3 getPosition() {
        return gameObject.transform.position;
    }

    //���������������������������������������������ܲ��֣���λ�ܹ����е���Ϊ��������������������������������������������������������������������������������
    /// <summary>
    /// ����Ŀ�귽��ǰ��
    /// </summary>
    /// <param name="targetPosition">Ŀ��λ��</param>
    public void MoveToWardTarget(Vector3 targetPosition) {
        Debug.Log("trying to moving!" + " Target: " + targetPosition);
        agent.SetDestination(targetPosition);
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
}
