using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitControl : MonoBehaviour
{
    private NavMeshAgent agent;

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

    /// <summary>
    /// ��������ߵ�ǰ��λ��λ����ʲô�ط���
    /// </summary>
    /// <returns>��Vector3���ص�xyzλ����Ϣ��</returns>
    public Vector3 getPosition() {
        return gameObject.transform.position;
    }

    /// <summary>
    /// ����Ŀ�귽��ǰ��
    /// </summary>
    /// <param name="targetPosition">Ŀ��λ��</param>
    public void MoveToWardTarget(Vector3 targetPosition) {
        Debug.Log("trying to moving!" + " Target: " + targetPosition);
        agent.SetDestination(targetPosition);
    }
}
