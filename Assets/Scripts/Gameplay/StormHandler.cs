using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormHandler : MonoBehaviour
{
    public int min_interval;
    public int max_interval;
    private int current_interval = 0;
    
    void Start()
    {
        current_interval = Random.Range(min_interval, max_interval);
        GameState.instance.turn_change_delegate += OnTurnChange;
    }

    void OnTurnChange(int turn_index)
    {
        current_interval--;
        if (current_interval <= 0)
        {
            if (!GridInstance.instance.GetCellContent(PlayerController.instance.player.cell).safe)
            {
                
            }
            current_interval = Random.Range(min_interval, max_interval);
        }
    }
}
