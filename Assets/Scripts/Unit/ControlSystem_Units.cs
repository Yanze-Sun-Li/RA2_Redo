using System.Collections.Generic;
using UnityEngine;

public class ControlSystem_Units : MonoBehaviour
{
    List<UnitControl> units;
    List<PlayerControl> players;

    // Start is called before the first frame update
    void Start()
    {
        AssignExitUnitsToPlayer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 注册一个新加入游戏场景的单位。
    /// </summary>
    /// <param name="unit">新加入游戏场景的单位</param>
    public void RegisterObject(UnitControl unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
            AssignUnitToPlayer(unit);
        }
    }

    /// <summary>
    /// 将一个单独的单位分配给现有的玩家
    /// </summary>
    /// <param name="unit">等待分配的单位</param>
    void AssignUnitToPlayer(UnitControl unit)
    {
        foreach (PlayerControl player in players)
        {
            if (player.playerID == -1)
            {
                Debug.LogError("One of the Players does not have a proper ID.");
                continue;
            }
            if (unit.playerID == player.playerID)
            {
                player.AddUnit(unit);
                break;
            }
        }
    }

    /// <summary>
    /// 将现有的所有单位分配给玩家。警告：如果仅需要分配单个单位，请不要重新分配所有单位。
    /// </summary>
    void AssignExitUnitsToPlayer()
    {
        units = new List<UnitControl>(FindObjectsOfType<UnitControl>());
        players = new List<PlayerControl>(FindObjectsOfType<PlayerControl>());

        foreach (UnitControl unit in units)
        {
            foreach (PlayerControl player in players)
            {
                if (player.playerID == -1)
                {
                    Debug.LogError("One of the Players does not have a proper ID. 某个玩家的初始ID是没有赋值的-1状态。");
                }
                if (unit.playerID == player.playerID)
                {
                    player.AddUnit(unit);
                }
            }
        }
    }
}
