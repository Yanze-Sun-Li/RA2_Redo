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
    /// ��������ߵ�ǰ��λ��λ����ʲô�ط���
    /// </summary>
    /// <returns>��Vector3���ص�xyzλ����Ϣ��</returns>
    public Vector3 getPosition() {
        return gameObject.transform.position;
    }
}
