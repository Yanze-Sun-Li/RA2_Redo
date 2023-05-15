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

    /// <summary>
    /// ��ǰ�������¼��ļ�ʱ�������ڽ��е��¼��Ƿ�ʱ��
    /// </summary>
    private void LeftMouseActionsTimer()
    {
        //������ڼȶ���ʱ������
        if (Time.time - timeRecord < timeLimitation && timeRecord != -1)
        {

        }
        // ����Ѿ������˹涨��ʱ������
        else if (Time.time - timeRecord > timeLimitation && timeRecord != -1)
        {
            EndLeftMouseActions();
        }
    }

    /// <summary>
    /// �������¼�������
    /// </summary>
    private void EndLeftMouseActions()
    {
        Debug.Log("���������¼���������ǰ�¼��ǣ�" + Time.time);
        Debug.Log("��갴�´�����"+ leftButtonDownCounter + "���̧�������"+leftButtonUpCounter);

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
                Debug.Log("���Ѿ�����������¼���ʼ��¼��");
                StartRecordingLeftMouseEvent();
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
        }
    }

    /// <summary>
    /// ����������ʼ��һ���¼���ʱ��
    /// </summary>
    private void StartRecordingLeftMouseEvent()
    {
        isPerformingAction = true;
        timeRecord = Time.time;
        Debug.Log("�¼��Ŀ�ʼʱ���ǣ�" + timeRecord);
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
        Debug.Log("��⵽������������");
    }
    /// <summary>
    /// ������˫���¼�����⵽��ʱ��
    /// </summary>
    private void LeftDoubleClickAction()
    {
        Debug.Log("��⵽������˫����");
    }
    /// <summary>
    /// �����������¼�����⵽��ʱ��
    /// </summary>
    private void LeftLongPressAction()
    {
        Debug.Log("��⵽������������");
    }





    private void Update()
    {
        LeftMouseActions();
        LeftMouseActionsTimer();
    }

}
