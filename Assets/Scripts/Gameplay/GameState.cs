using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate IEnumerator CoroutineDelegate();

public class GameState : MonoBehaviour
{
    public static GameState instance;

    public int turn_index = 0;
    public CoroutineDelegate turn_change_delegate;
    public System.Action phase_reset_delegate;

    void Awake()
    {
        instance = this;
    }

    public void FinishTurn()
    {
        StartCoroutine(FinishTurnCoroutine());
    }

    public IEnumerator FinishTurnCoroutine()
    {
        turn_index++;
        List<Coroutine> coroutines = new List<Coroutine>();
        foreach (Delegate invoc in turn_change_delegate.GetInvocationList())
        {
            IEnumerator coroutine = (invoc as CoroutineDelegate)();
            coroutines.Add(StartCoroutine(coroutine));
        }

        foreach (Coroutine coroutine in coroutines)
        {
            yield return coroutine;
        }
    }
}
