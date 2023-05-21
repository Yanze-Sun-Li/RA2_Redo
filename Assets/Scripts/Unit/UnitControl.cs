using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitControl : MonoBehaviour
{
    //！！！！！！！！！！！！！！！！！！！！！！！！！佚連窃！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
    private NavMeshAgent agent;
    //！！！！！！！！！！！！！！！！！！！！！！！！！奉來窃！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
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


    //！！！！！！！！！！！！！！！！！！！！！！喘噐�鯑篷�議Component戻工佚連議孔嬬！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
    /// <summary>
    /// 公嚠恵諒宀輝念汽了議了崔壓焚担仇圭。
    /// </summary>
    /// <returns>參Vector3卦指議xyz了崔佚連。</returns>
    public Vector3 getPosition() {
        return gameObject.transform.position;
    }

    //！！！！！！！！！！！！！！！！！！！！！孔嬬何蛍��汽了嬬校序佩議佩葎！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
    /// <summary>
    /// �鰈田娠蠏熟鯒綾�
    /// </summary>
    /// <param name="targetPosition">朕炎了崔</param>
    public void MoveToWardTarget(Vector3 targetPosition) {
        Debug.Log("trying to moving!" + " Target: " + targetPosition);
        agent.SetDestination(targetPosition);
    }

    //！！！！！！！！！！！！！！！！！！！！！兜兵晒��潮範譜崔泡仟吉�犢惺δ棔�！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
    /// <summary>
    /// 繍汽了佚連泡仟葎兜兵彜蓑。
    /// </summary>
    public void InfoInitial() 
    {
        id = -1;
        playerID = -1;
        name = null;
        tag = null;

    }
}
