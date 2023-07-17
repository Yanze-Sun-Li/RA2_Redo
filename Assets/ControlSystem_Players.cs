using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSystem_Players : MonoBehaviour
{
    public List<PlayerControl> players;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToPlayerList(PlayerControl player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }

    public void RemoveFromPlayerList(PlayerControl player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }
}
