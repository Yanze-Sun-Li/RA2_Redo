using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ܣ�
///     Ŀǰ���ڶ���ҵ����в���������Ӧ��
/// ���ܣ�
///     ��ȡ��������������������˫��
/// </summary>
public class PlayerControl : MonoBehaviour
{
    //�������������������������������������������������ӵ�е���Դ�����Եȡ���������������������������������������������������������������������������������������������������������
    /* ע�⣺�˴��д��Ż����ܵ����󣡣� 
     * ʹ��List����ɵ�ʱ�򷽱㣬�����ڴ�������ĵ�λ��ʱ��ʹ��List��¼PlayerUnit����Ҫ��Quadtree��������ܡ�
     * ��Ϊ�����ҵ�ʱ��list��������Ҫ�����������д����playerUnit
     * ��������listӵ���Լ�����ɫ��Ҳ�������ڴ�ռ��㹻������£�ͬʱʹ��List��Quadtree�������ݼ�¼����Ӧ����ͬ״���µ�����
     */

    [SerializeField]
    private List<UnitControl> playerUnits;
    
    [Header("Debug��Ϣ")]
    [SerializeField]
    private List<UnitControl> playerSelectedUnits;

    //______________________�����������صĴ���____________________________________________
    /* ��һ����ʱ���ڣ����㰴�º�̧�����Ŀ��ʹ���жϷ������¼�����֮�󣬸�����ҽ�������������Ϊ��
     * �������ڹ涨ʱ���ڵ����������е���ѡ����λ����ָ�
     * �������ڹ涨ʱ����˫���������е���ѡ����λ����ָ�
     * �������ڹ涨ʱ���ڳ��������п�ѡ��Ϊ��
     */
    //------------------���ڼ���ʱ���¼�ı����ͷ����б�����----------------------
    float timeRecord = -1;
    //�涨��ʱ���ж����
    [SerializeField]
    private float timeLimitation = .3f;
    bool isPerformingAction = false;

    float timeHoldingRecord = -1;
    [SerializeField]
    private float timeLimitation_Holding = .1f;

    /// <summary>
    /// ��ǰ�������¼��ļ�ʱ�������ڽ��е��¼��Ƿ�ʱ��
    /// ע�⣺����ס��ʱ����ǰ�����¼���
    /// </summary>
    private void LeftMouseActionsTimer()
    {
        if (isPerformingAction)
        {
            //������ڼȶ���ʱ������
            if (Time.time - timeRecord < timeLimitation && timeRecord != -1)
            {
                
            }// ����Ѿ������˹涨��ʱ������
            else if (Time.time - timeRecord > timeLimitation && timeRecord != -1)
            {
                // Debug.log("ʱ�������");
                EndLeftMouseActions();
                return;
            }

            // ���������ʱ�䳬���˹涨ʱ������
            if (Time.time - timeHoldingRecord > timeLimitation_Holding && timeHoldingRecord != -1)
            {
                // Debug.log("������ʱ�������");
                EndLeftMouseActions();
                return;
            }
            
        }
    }

    /// <summary>
    /// �������¼�������
    /// </summary>
    private void EndLeftMouseActions()
    {
        //���¼�����֮�󣬶���ҽ��е��¼������ж�����ҽ����˵���������˫�����ֻ����ǳ�����
        DetermineLeftMouseAction();

        if (ifLeftButtonSingleClickedEvent)
        {
            LeftSingleClickAction();
            ifLeftButtonSingleClickedEvent = false;
        } else if (ifLeftButtonDoubleClickedEvent)
        {
            LeftDoubleClickAction();
            ifLeftButtonDoubleClickedEvent = false;
        } else if (ifLeftButtonLongPressEvent) {
            //�����Ҫ��������������¼�����ʱ���������뽫����������·����
            LeftLongPressAction();
            ifLeftButtonLongPressEvent = false;
        }

        //��������صı�������ΪĬ��ֵ
        LeftButtonCountersRenew();
    }
    //------------------���ڼ���������¼��ı����ͷ����б����£�-------------
    int leftButtonDownCounter = 0;
    int leftButtonUpCounter = 0;

    //����Ƿ���������������
    bool ifLeftButtonLongPressing = false;
    //����Ƿ�Ҫ������������¼�
    bool ifLeftButtonLongPressEvent = false;
    
    bool ifLeftButtonSingleClickedEvent = false;
    bool ifLeftButtonDoubleClickedEvent = false;


    /// <summary>
    /// ����������������¼�
    /// </summary>
    void LeftMouseActions() 
    {
        //���������������
        if (LeftButtonDown()) {
            leftButtonDownCounter++;
            ifLeftButtonLongPressing = true;
            //�����ǰ��û�����ڽ��е�����¼��ж������ڿ�ʼ��¼�¼��Ŀ�ʼ��
            if (!isPerformingAction)
            {
                // Debug.log("���Ѿ�����������¼���ʼ��¼��");
                StartRecordingLeftMouseEvent();
                //��¼��갴ѹ��ʱ��
                timeHoldingRecord = Time.time;
            }
            else 
            {
            
            }
        }
        //���������̧����
        if (LeftButtonUp())
        {
            ifLeftButtonLongPressing = false;
            if (isPerformingAction) 
            {
                leftButtonUpCounter++;
            }
            if (isRecordingMousePosition)
            {
                RecordingReleaseMousePosion_OnScreen();
                LeftLongPressActionOnFinish();
                isRecordingMousePosition = false;
            }
        }
    }

    /// <summary>
    /// ����������ʼ��һ���¼���ʱ��
    /// </summary>
    private void StartRecordingLeftMouseEvent()
    {
        isPerformingAction = true;
        timeRecord = Time.time;
        // Debug.log("�¼��Ŀ�ʼʱ���ǣ�" + timeRecord);
        RecordingClickPosition_OnScreen();
    }

    /// <returns>�Ƿ��������ť��</returns>
    private bool LeftButtonDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    /// <returns>�Ƿ�̧�������ť��</returns>
    private bool LeftButtonUp()
    {
        return Input.GetMouseButtonUp(0);
    }

    /// <summary>
    /// ��������������¼���صı�����������ĳ�ʼ״̬
    /// </summary>
    private void LeftButtonCountersRenew() 
    {
        leftButtonDownCounter = 0;
        leftButtonUpCounter = 0;
        timeRecord = -1;
        isPerformingAction = false;
        timeHoldingRecord = -1;
    }



    //------------------�����ж������е��������¼����͵ı����ͷ����б����£�-------------
    /// <summary>
    /// �����ʱ�������ڣ�ִ�����������¼���
    /// </summary>
    private void DetermineLeftMouseAction()
    {
        if (leftButtonDownCounter == 1 && leftButtonUpCounter == 1)
        {
            ifLeftButtonSingleClickedEvent = true;
        }
        // ע�� ���������趨������ڹ涨ʱ�����ڵ���������ڣ����߳���2�Σ��Ҷ�����Ϊ��˫����
        else if (leftButtonDownCounter >= 2 && leftButtonDownCounter == leftButtonUpCounter)
        {
            ifLeftButtonDoubleClickedEvent = true;
        } else if (leftButtonUpCounter == 0) {
            ifLeftButtonLongPressEvent = true;
        }
    }
    /// <summary>
    /// �����������¼�����⵽��ʱ��
    /// </summary>
    private void LeftSingleClickAction()
    {
        // Debug.log("��⵽������������");
    }
    /// <summary>
    /// ������˫���¼�����⵽��ʱ��
    /// </summary>
    private void LeftDoubleClickAction()
    {
        // Debug.log("��⵽������˫����");
    }
    /// <summary>
    /// �����������¼�����⵽��ʱ��
    /// </summary>
    private void LeftLongPressAction()
    {
        // Debug.log("��⵽������������");
        isRecordingMousePosition = true;

    }
    private void LeftLongPressActionOnFinish() 
    {
        BoxSelecting();
    }


    //______________________��ѡ���ܵĴ���____________________________________________
    [SerializeField]
    Vector3 eventClickPosition;
    [SerializeField]
    Vector3 eventReleasePosition;
    [SerializeField]
    private Camera mainCamera;
    bool isRecordingMousePosition = false;
    public LayerMask unitLayer; // ��λ��ͼ��
    [SerializeField]


    /// <summary>
    /// ���ص�ǰ���λ������Ϸ�������ڴ����Vector3 Postionλ����ֵ��
    /// ע�⣺������Ļ�����λ�ã�������Ϸ�����ڴ�ʱ�����ָλ�á�
    /// </summary>
    /// <returns></returns>
    Vector3 ReturnHitPosition() {
        Vector3 clickPosition = new Vector3(0f,0f,0f);
        // ����һ���������λ������Ļ���������
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // ��������Ƿ��������ײ
        if (Physics.Raycast(ray, out hit))
        {
            // ����Ƿ���������"Ground"��ǩ������
            if (hit.collider.CompareTag("Ground")) {

                // ��ȡ��ײ�������
                clickPosition = hit.point;

                // ��������ԶԵ��λ�ý��д���
                // �����ڸ�λ�ô�����λ�������������
                // Debug.log("Clicked position: " + clickPosition);
                return clickPosition;
            }
            //else { 
            
            //}
        }

        return Vector3.zero;
        
    }
    /// <summary>
    /// ��¼�����̧��ʱ�����ͣ����λ��
    /// </summary>

    void RecordingClickPosition_OnScreen()
    {
        eventClickPosition = Input.mousePosition;
    }
    void RecordingReleaseMousePosion_OnScreen() 
    {
        eventReleasePosition = Input.mousePosition;
    }

    //��������������������������������������������������������������������������������������ѡ������������������������������������������������������������������������
    Rect screenSelectRect;

    /// <summary>
    /// ��ѡ����Ҫ����
    /// </summary>
    void BoxSelecting()
    {
        //foreach ����ÿһ����ҿɿ��Ƶ�Unit
        if (playerUnits.Count != 0) {
            playerSelectedUnits.Clear();
            foreach (UnitControl unit in playerUnits)
            {
                //���ж�������û���ڷ�Χ�ڡ�
                if (UnitInTheArea(unit.getPosition()))
                {
                    playerSelectedUnits.Add(unit);
                    //Debug.log("��Ʒ�Ѿ���ӡ�");
                }
                else
                {

                }
                
            }
        }
       

    }

    /// <summary>
    /// ���ڵ�ǰ���������λ�ã��ж����Ƿ����������Ļ�Ͽ�ѡ֮�ڡ�
    /// </summary>
    /// <param name="position">��ǰ�����λ����Ϣ</param>
    /// <returns>���ص�ǰ�ĵ�λ�Ƿ�����ҵĿ�ѡ��Χ֮��</returns>
    private bool UnitInTheArea(Vector3 position)
    {
        if (InSelectArea(position))
        {
            //Debug.log("��ǰ�����ڿ�ѡ��Χ֮�ڡ� ");
            return true;
        }
        else {
            //Debug.log("��ǰ���岻�ڿ�ѡ��Χ֮�ڡ� ");
            return false;
        }
    }

    /// <summary>
    /// ����������λ����Ϣ�ڿ�ѡ�ķ�Χ֮�ڣ�
    /// </summary>
    /// <param name="onCameraPosition">����λ��</param>
    /// <returns>�Ƿ��ڿ�ѡ��λ��֮�ڣ�</returns>
    private bool InSelectArea(Vector3 onCameraPosition)
    {
        //Debug.log(ConvertToScreenPosition(onCameraPosition));
        return screenSelectRect.Contains(ConvertToScreenPosition(onCameraPosition));
    }

    Vector3 ConvertToScreenPosition(Vector3 gamePosition)
    {
        // ����������ת��Ϊ��Ļ����
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(gamePosition);

        // Unity ��Ļ����ϵ�� Y ���Ƿ���ģ���Ҫ��������
        screenPosition.y = Screen.height - screenPosition.y;

        return screenPosition;
    }

    /// <summary>
    /// ����Ļ�ϻ���һ�����Σ�����ʾ��ѡ��ʵ�ʷ�Χ��
    /// </summary>
    /// <param name="startPos">��ʲô�ط���ʼ����</param>
    /// <param name="endPos">��ʲô�ط�������</param>
    /// <returns></returns>
    private Rect GetScreenRect(Vector3 startPos, Vector3 endPos)
    {
        // ������ε����Ͻ�λ�úͿ��
        Vector2 min = Vector2.Min(startPos, endPos);
        Vector2 max = Vector2.Max(startPos, endPos);
        return new Rect(min.x, Screen.height - max.y, max.x - min.x, max.y - min.y);
    }


    //������������������������������������������������������������������������������������������������������������ Unity ���� ������������������������������������������������������������������������������������

    private void Start()
    {
        mainCamera = Camera.main;
        if (playerUnits.Count == 0)
        {
            playerUnits = new List<UnitControl>();
        }
        playerSelectedUnits = new List<UnitControl>();
    }
    private void Update()
    {
        LeftMouseActions();
        LeftMouseActionsTimer();

    }

    private void OnGUI()
    {
        if (isRecordingMousePosition)
        {

            // ���ƿ�ѡ����
            screenSelectRect = GetScreenRect(eventClickPosition, Input.mousePosition);
            GUI.Box(screenSelectRect, "");

        }
    }

}

