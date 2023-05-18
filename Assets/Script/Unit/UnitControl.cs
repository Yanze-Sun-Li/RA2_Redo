using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
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
}
