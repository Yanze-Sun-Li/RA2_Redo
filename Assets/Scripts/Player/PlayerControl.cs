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
    

    //——————————————————————玩家所拥有的资源和属性等—————————————————————————————————————————————————————
    /* 注意：此处有待优化性能的需求！！ 
     * 使用List在完成的时候方便，但是在处理大量的单位的时候，使用List记录PlayerUnit会需要比Quadtree更多的性能。
     * 因为当查找的时候，list往往会需要历经其中所有储存的playerUnit
     * 不过由于list拥有自己的特色，也可以在内存空间足够的情况下，同时使用List和Quadtree来做两份记录，以应付不同状况下的需求。
     */

    [SerializeField]
    private List<UnitControl> playerUnits;

    /*
     * ——————————————————————重要信息，玩家如何在出场时正确判断自己的部队——————————————————————————————————————————
     * 每一位PlayerControl都应该有一个独属于玩家的ID。在游戏开始的时候，会访问场上所有的单位，并根据玩家的ID将单位加入到对应玩家的阵营里。
     * 目前在草案中预设中立单位的playerID为0，请保持玩家的ID从1起步。
     */
    [SerializeField]
    public int playerID = -1;
    
    [Header("Debug信息")]
    [SerializeField]
    private List<UnitControl> playerSelectedUnits;

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

    float timeHoldingRecord = -1;
    [SerializeField]
    private float timeLimitation_Holding = .2f;

    /// <summary>
    /// 当前左键鼠标事件的计时器，正在进行的事件是否超时？
    /// 注意：当按住的时候，提前结束事件。
    /// </summary>
    private void LeftMouseActionsTimer()
    {
        if (isPerformingAction)
        {
            //如果还在既定的时间以内
            if (Time.time - timeRecord < timeLimitation && timeRecord != -1)
            {
                
            }// 如果已经超过了规定的时间限制
            else if (Time.time - timeRecord > timeLimitation && timeRecord != -1)
            {
                // Debug.log("时间结束！");
                EndLeftMouseActions();
                return;
            }

            // 如果长按的时间超过了规定时间限制
            if (Time.time - timeHoldingRecord > timeLimitation_Holding && timeHoldingRecord != -1)
            {
                // Debug.log("长按的时间结束！");
                EndLeftMouseActions();
                return;
            }
            
        }
    }

    //______________________玩家鼠标左键检测相关的代码____________________________________________

    /// <summary>
    /// 当鼠标左键事件结束之后...
    /// </summary>
    private void EndLeftMouseActions()
    {
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
                RecordingClickPosition_OnScreen();
                // Debug.log("你已经按下左键，事件开始记录。");
                StartRecordingLeftMouseEvent();
                //记录鼠标按压的时间
                timeHoldingRecord = Time.time;
            }
            else 
            {
            
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
            if (isRecordingMousePosition)
            {
                RecordingReleaseMousePosion_OnScreen();
                LeftLongPressActionOnFinish();
                isRecordingMousePosition = false;
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
        // Debug.log("事件的开始时间是：" + timeRecord);
        //RecordingClickPosition_OnScreen();
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
        timeHoldingRecord = -1;
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
        // Debug.log("检测到鼠标左键单击。");

        if (playerSelectedUnits.Count == 0)
        {
            ResetHitValue();
            UnitControl unit = ReturnHitUnit();
            if (isHitUnit)
            {
                if (unit.playerID == playerID)
                {
                    playerSelectedUnits.Add(unit);
                }
            }
        }
        else if (playerSelectedUnits.Count > 0)
        {
            ResetHitValue();
            //————————————————这个地方有进行优化的空间，但是由于时间关系，这里暂且先不做调整————————————————————
            //会再次重复的发出raycast，但是由于数目极小，对性能影响微弱，暂且不做调整
            UnitControl unit = ReturnHitUnit();
            Vector3 position = ReturnHitGroundPosition();
            if (isHitUnit)
            {
                if (unit.playerID == playerID)
                {
                    //如果击中地面，要求目标移动到指定位置。
                    UnitMoving();
                }
                else
                {
                    foreach (UnitControl selectedUnits in playerSelectedUnits)
                    {
                        selectedUnits.enemyUnitTarget = selectedUnits;
                        selectedUnits.AttackLogic();
                    }
                    Debug.Log("攻击指定目标！" + unit.name);
                }
            }
            else if (isHitGround)
            {
                UnitMoving();
            }
        }
        else {
            Debug.LogError("Player selected units amount should not under 0! 玩家已选择的单位数目不应该低于0！");
        }

    }
    /// <summary>
    /// 鼠标左键双击事件被检测到的时候
    /// </summary>
    private void LeftDoubleClickAction()
    {
        // Debug.log("检测到鼠标左键双击。");
    }
    /// <summary>
    /// 鼠标左键长按事件被检测到的时候
    /// </summary>
    private void LeftLongPressAction()
    {
        // Debug.log("检测到鼠标左键长按。");
        isRecordingMousePosition = true;

    }
    private void LeftLongPressActionOnFinish() 
    {
        BoxSelecting();
    }

    //______________________框选功能的代码____________________________________________
    [SerializeField]
    Vector3 eventClickPosition;
    [SerializeField]
    Vector3 eventReleasePosition;
    [SerializeField]
    private Camera mainCamera;
    bool isRecordingMousePosition = false;
    public LayerMask unitLayer; // 单位的图层
    [SerializeField]

    /// <summary>
    /// 记录下鼠标抬起时候，鼠标停留的位置
    /// </summary>

    void RecordingClickPosition_OnScreen()
    {
        eventClickPosition = Input.mousePosition;
    }
    void RecordingReleaseMousePosion_OnScreen() 
    {
        eventReleasePosition = Input.mousePosition;
    }

    //——————————————————————————————————————————框选————————————————————————————————————
    Rect screenSelectRect;

    /// <summary>
    /// 框选的主要功能
    /// </summary>
    void BoxSelecting()
    {
        //foreach 经过每一个玩家可控制的Unit
        if (playerUnits.Count != 0) {
            playerSelectedUnits.Clear();
            foreach (UnitControl unit in playerUnits)
            {
                //来判断物体有没有在范围内。
                if (UnitInTheArea(unit.getPosition()))
                {
                    playerSelectedUnits.Add(unit);
                    //Debug.log("物品已经添加。");
                }
                else
                {

                }
                
            }
        }
       

    }

    /// <summary>
    /// 基于当前物体的所处位置，判断它是否在玩家在屏幕上框选之内。
    /// </summary>
    /// <param name="position">当前物体的位置信息</param>
    /// <returns>返回当前的单位是否在玩家的框选范围之内</returns>
    private bool UnitInTheArea(Vector3 position)
    {
        if (InSelectArea(position))
        {
            //Debug.log("当前物体在框选范围之内。 ");
            return true;
        }
        else {
            //Debug.log("当前物体不在框选范围之内。 ");
            return false;
        }
    }

    /// <summary>
    /// 如果被给予的位置信息在框选的范围之内？
    /// </summary>
    /// <param name="onCameraPosition">给予位置</param>
    /// <returns>是否在框选的位置之内？</returns>
    private bool InSelectArea(Vector3 onCameraPosition)
    {
        //Debug.log(ConvertToScreenPosition(onCameraPosition));
        return screenSelectRect.Contains(ConvertToScreenPosition(onCameraPosition));
    }

    Vector3 ConvertToScreenPosition(Vector3 gamePosition)
    {
        // 将世界坐标转换为屏幕坐标
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(gamePosition);

        // Unity 屏幕坐标系的 Y 轴是反向的，需要进行修正
        screenPosition.y = Screen.height - screenPosition.y;

        return screenPosition;
    }

    /// <summary>
    /// 在屏幕上画出一个矩形，来表示框选的实际范围。
    /// </summary>
    /// <param name="startPos">从什么地方开始画？</param>
    /// <param name="endPos">从什么地方结束？</param>
    /// <returns></returns>
    private Rect GetScreenRect(Vector3 startPos, Vector3 endPos)
    {
        // 计算矩形的左上角位置和宽高
        Vector2 min = Vector2.Min(startPos, endPos);
        Vector2 max = Vector2.Max(startPos, endPos);
        return new Rect(min.x, Screen.height - max.y, max.x - min.x, max.y - min.y);
    }

    //—————————————————————————————————————————————————————— 左键行为 ——————————————————————————————————————————
    //指示单位向着目标方向前进。
    private void UnitMoving() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 检测到地面命中
            if (hit.collider.CompareTag("Ground"))
            {
                Vector3 groundPosition = hit.point;
                TeamOrdering(groundPosition);
            }
        }
    }
    //—————————————————————————————————————————————————————— 右键行为 ——————————————————————————————————————————
    
    //______________________玩家鼠标右键检测相关的代码____________________________________________
    /// <summary>
    /// 由于开发者的极度懒惰行为，目前只检测右键单击行为。
    /// </summary>
    void RightMouseActions()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RightSingleClickAction();
        }
    }

    //———————————鼠标右键的事件方法————————————————————————————
    /// <summary>
    /// 当检测到鼠标右键活动之后，激活该方法。
    /// </summary>
    private void RightSingleClickAction()
    {
        playerSelectedUnits.Clear();
    }


    //———————————————————————————————————————————————————————与其他物体的组件进行沟通的公共方法————————————————————————————————————
    public void AddUnit(UnitControl unit) 
    { 
        playerUnits.Add(unit);
    }

    //—————————————————————————————————————————————————获取当前鼠标的目标—————————————————————————————————————
    /*
     * 我认为一下的Hit系列功能可以优化，整合成为单独一个判断方法。 如果整合之后，左键的单击判断敌人，地面和友军的逻辑也可以进行优化。
     * 但是因为时间的问题，作者将暂时不对这里进行优化修改。
     */
    
    //判断目标的种类。
    bool isHitGround = false;
    bool isHitUnit = false;
    bool isHitBuilding = false;
    /// <summary>
    /// 返回当前鼠标位置在游戏的世界内代表的Vector3 Postion位置数值。
    /// 注意：不是屏幕上鼠标位置，而是游戏世界内此时鼠标所指位置。 
    /// 注意：如果集中地面，isHitGround会显示为True
    /// </summary>
    /// <returns>当前鼠标在游戏世界里的地面位置</returns>
    Vector3 ReturnHitGroundPosition()
    {
        Vector3 clickPosition = new Vector3(0f, 0f, 0f);
        // 创建一条从鼠标点击位置向屏幕发射的射线
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 检测射线是否与地面碰撞
        if (Physics.Raycast(ray, out hit))
        {
            // 检查是否点击到带有"Ground"标签的物体
            if (hit.collider.CompareTag("Ground"))
            {

                // 获取碰撞点的坐标
                clickPosition = hit.point;

                //判断鼠标的位置在地面上
                isHitGround= true;

                // 在这里可以对点击位置进行处理
                // 例如在该位置创建单位或进行其他操作
                // Debug.log("Clicked position: " + clickPosition);
                return clickPosition;
            }
            //else { 

            //}
        }

        return Vector3.zero;

    }

    /// <summary>
    /// 返回当前鼠标位置在游戏的世界内代表的Vector3 Postion位置数值。
    /// 注意：不是屏幕上鼠标位置，而是游戏世界内此时鼠标所指位置。 
    /// 注意：如果集中地面，isHitGround会显示为True
    /// </summary>
    /// <returns>当前鼠标在游戏世界里的地面位置</returns>
    UnitControl ReturnHitUnit()
    {
        UnitControl unit;
        // 创建一条从鼠标点击位置向屏幕发射的射线
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 检测射线是否与地面碰撞
        if (Physics.Raycast(ray, out hit))
        {
            // 检查是否点击到带有"Ground"标签的物体
            if (hit.collider.CompareTag("Units"))
            {

                // 获取碰撞点的坐标
                unit = hit.collider.gameObject.GetComponent<UnitControl>();

                //判断鼠标的位置在地面上
                isHitUnit = true;

                // 在这里可以对点击位置进行处理
                // 例如在该位置创建单位或进行其他操作
                // Debug.log("Clicked position: " + clickPosition);
                return unit;
            }
            //else { 

            //}
        }

        return null;

    }

    
    /// <summary>
    /// 重置击中判断变量为初始false状态。
    /// </summary>
    private void ResetHitValue() 
    {
        isHitBuilding = false;
        isHitGround= false;
        isHitUnit= false;
    }
    //——————————————————————————————————————————————————————— 列队功能 ————————————————————————————————————————————
    //每一个列队的最大成员数字，（默认设置3*3）用于计算列队的行数。
    public float maxUnits = 1;
    public float offsetX = 2f; // 单位之间的水平偏移量
    public float offsetZ = 2f; // 单位之间的垂直偏移量

    ///// <summary>
    ///// 列队前行的主要功能
    ///// 计算队伍的数目，尽量的形成列队
    ///// </summary>
    //private void TeamOrdering(Vector3 destination_targetPosition) {
    //    // 假设 maxUnits 是每个队伍的最大单位数
    //    int unitCount = playerSelectedUnits.Count; // 初始单位数量等于已选中列表中的单位数量
    //    int rows = Mathf.CeilToInt(Mathf.Sqrt(unitCount / maxUnits)); // 计算行数
    //    int cols = Mathf.CeilToInt(unitCount / (float)rows); // 计算列数


    //    for (int i = 0; i < unitCount; i++)
    //    {
    //        int row = i / cols; // 计算单位所在的行索引
    //        int col = i % cols; // 计算单位所在的列索引

    //        float posX = col * offsetX; // 计算单位的水平位置
    //        float posZ = row * offsetZ; // 计算单位的垂直位置

    //        // 设置单位的目标位置为列队的位置
    //        Vector3 targetPosition = new Vector3(posX, 0f, posZ) + destination_targetPosition;
    //        playerSelectedUnits[i].MoveToWardTargetTrigger(targetPosition);
    //    }
    //}

    /// <summary>
    /// 列队前行的主要功能
    /// 计算队伍的数目，尽量的形成列队
    /// </summary>
    private void TeamOrdering(Vector3 destination_targetPosition)
    {
        int rows = Mathf.CeilToInt(Mathf.Sqrt(playerSelectedUnits.Count)); // 计算行数
        int cols = rows; // 列数与行数相等

        for (int i = 0; i < playerSelectedUnits.Count; i++)
        {
            int row = i / cols; // 计算单位所在的行索引
            int col = i % cols; // 计算单位所在的列索引

            float posX = col * offsetX; // 计算单位的水平位置
            float posZ = row * offsetZ; // 计算单位的垂直位置

            // 设置单位的目标位置为列队的位置
            Vector3 targetPosition = new Vector3(posX, 0f, posZ) + destination_targetPosition;
            playerSelectedUnits[i].MoveToWardTargetTrigger(targetPosition);
        }

    }
    //—————————————————————————————————————————————————————— Unity 方法 ——————————————————————————————————————————

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
        RightMouseActions();
    }

    private void OnGUI()
    {
        if (isRecordingMousePosition)
        {

            // 绘制框选矩形
            screenSelectRect = GetScreenRect(eventClickPosition, Input.mousePosition);
            GUI.Box(screenSelectRect, "");

        }
    }

}

