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
    /// 给予访问者当前单位的位置在什么地方。
    /// </summary>
    /// <returns>以Vector3返回的xyz位置信息。</returns>
    public Vector3 getPosition() {
        return gameObject.transform.position;
    }

    /// <summary>
    /// 向着目标方向前进
    /// </summary>
    /// <param name="targetPosition">目标位置</param>
    public void MoveToWardTarget(Vector3 targetPosition) {
        Debug.Log("trying to moving!" + " Target: " + targetPosition);
        agent.SetDestination(targetPosition);
    }
}
