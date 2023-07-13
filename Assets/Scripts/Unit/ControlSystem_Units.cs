using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ControlSystem_Units : MonoBehaviour
{
    UnitControl[] units;
    PlayerControl[] players;

    // Start is called before the first frame update
    void Start()
    {
        AssignExitUnitToPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AssignExitUnitToPlayer() {
        units = FindObjectsOfType<UnitControl>();
        players = FindObjectsOfType<PlayerControl>();

        foreach (UnitControl unit in units)
        {
            foreach (PlayerControl player in players)
            {
                if (player.playerID == -1)
                {
                    Debug.LogError("One of the Player do not have an proper ID. ÓÐÒ»Î»Íæ¼ÒµÄÓÃ»§IDÃ»ÓÐ³õÊ¼»¯¡£");
                }
                if (unit.playerID == player.playerID)
                {
                    player.AddUnit(unit);
                }
            }
        }
    }
}
