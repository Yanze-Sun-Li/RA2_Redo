using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 介绍：
///     目前用于对玩家的所有操作做出回应。
/// 功能：
///     读取玩家鼠标左键点击，长按，双击
/// </summary>
public class PlayerControl : MonoBehaviour
{



    //______________________玩家鼠标左键相关的代码____________________________________________
    /* 在一定的时间内，计算按下和抬起的数目，使用判断方法在事件结束之后，给出玩家进行了怎样的行为。
     * 如果玩家在规定时间内单击，对所有的已选定单位发出指令。
     * 如果玩家在规定时间内双击，对所有的已选定单位发出指令。
     * 如果玩家在规定时间内长按，进行框选行为。
     */
    //------------------用于计算时间纪录的变量和方法列表如下----------------------
    float timeRecord = -1;
    //规定的时间判定间隔
    [SerializeField]
    private float timeLimitation = .3f;
    bool isPerformingAction = false;

    /// <summary>
    /// 当前左键鼠标事件的计时器，正在进行的事件是否超时？
    /// </summary>
    private void LeftMouseActionsTimer()
    {
        //如果还在既定的时间以内
        if (Time.time - timeRecord < timeLimitation && timeRecord != -1)
        {

        }
        // 如果已经超过了规定的时间限制
        else if (Time.time - timeRecord > timeLimitation && timeRecord != -1)
        {
            EndLeftMouseActions();
        }
    }

    /// <summary>
    /// 鼠标左键事件结束。
    /// </summary>
    private void EndLeftMouseActions()
    {
        Debug.Log("鼠标左键的事件结束。当前事件是：" + Time.time);
        Debug.Log("鼠标按下次数："+ leftButtonDownCounter + "鼠标抬起次数："+leftButtonUpCounter);

        //在事件结束之后，对玩家进行的事件进行判定，玩家进行了单击，还是双击？又或者是长按？
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
            //如果想要代码在左键长按事件出现时被触发，请将代码放在以下方法里。
            LeftLongPressAction();
            ifLeftButtonLongPressEvent = false;
        }





        //将所有相关的变量更改为默认值
        LeftButtonCountersRenew();
    }
    //------------------用于检测鼠标左键事件的变量和方法列表如下：-------------
    int leftButtonDownCounter = 0;
    int leftButtonUpCounter = 0;

    //检测是否正在左键长按鼠标
    bool ifLeftButtonLongPressing = false;
    //检测是否要触发左键长按事件
    bool ifLeftButtonLongPressEvent = false;
    
    bool ifLeftButtonSingleClickedEvent = false;
    bool ifLeftButtonDoubleClickedEvent = false;


    /// <summary>
    /// 检测鼠标左键产生的事件
    /// </summary>
    void LeftMouseActions() 
    {
        //如果鼠标左键按下了
        if (LeftButtonDown()) {
            leftButtonDownCounter++;
            ifLeftButtonLongPressing = true;
            //如果当前并没有正在进行的鼠标事件判定，现在开始记录事件的开始。
            if (!isPerformingAction)
            {
                Debug.Log("你已经按下左键，事件开始记录。");
                StartRecordingLeftMouseEvent();
            }
        }
        //如果鼠标左键抬起了
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
    /// 当鼠标左键开始了一个事件的时候
    /// </summary>
    private void StartRecordingLeftMouseEvent()
    {
        isPerformingAction = true;
        timeRecord = Time.time;
        Debug.Log("事件的开始时间是：" + timeRecord);
    }

    /// <returns>是否按下左键按钮？</returns>
    private bool LeftButtonDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    /// <returns>是否抬起左键按钮？</returns>
    private bool LeftButtonUp()
    {
        return Input.GetMouseButtonUp(0);
    }

    /// <summary>
    /// 重制所有与左键事件相关的变量，到最初的初始状态
    /// </summary>
    private void LeftButtonCountersRenew() 
    {
        leftButtonDownCounter = 0;
        leftButtonUpCounter = 0;
        timeRecord = -1;
        isPerformingAction = false;
    }



    //------------------用于判断所进行的鼠标左键事件类型的变量和方法列表如下：-------------
    /// <summary>
    /// 玩家在时间限制内，执行了怎样的事件？
    /// </summary>
    private void DetermineLeftMouseAction()
    {
        if (leftButtonDownCounter == 1 && leftButtonUpCounter == 1)
        {
            ifLeftButtonSingleClickedEvent = true;
        }
        // 注： 我在这里设定，如果在规定时间以内点击次数等于，或者超过2次，我都设置为了双击。
        else if (leftButtonDownCounter >= 2 && leftButtonDownCounter == leftButtonUpCounter)
        {
            ifLeftButtonDoubleClickedEvent = true;
        } else if (leftButtonUpCounter == 0) {
            ifLeftButtonLongPressEvent = true;
        }
    }
    /// <summary>
    /// 鼠标左键单击事件被检测到的时候
    /// </summary>
    private void LeftSingleClickAction()
    {
        Debug.Log("检测到鼠标左键单击。");
    }
    /// <summary>
    /// 鼠标左键双击事件被检测到的时候
    /// </summary>
    private void LeftDoubleClickAction()
    {
        Debug.Log("检测到鼠标左键双击。");
    }
    /// <summary>
    /// 鼠标左键长按事件被检测到的时候
    /// </summary>
    private void LeftLongPressAction()
    {
        Debug.Log("检测到鼠标左键长按。");
    }





    private void Update()
    {
        LeftMouseActions();
        LeftMouseActionsTimer();
    }

}
