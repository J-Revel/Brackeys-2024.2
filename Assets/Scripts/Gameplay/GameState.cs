using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState instance;

    public int turn_index = 0;
    public System.Action<int> turn_change_delegate;

    void Start()
    {
        instance = this;
    }

    public void NextTurn()
    {
        turn_index++;
        turn_change_delegate?.Invoke(turn_index);
    }
}
