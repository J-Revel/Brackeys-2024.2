using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameoverMenu : MonoBehaviour
{
    private CanvasGroup canvas_group;
    public float appear_duration = 2;
    public float disappear_duration = 1;

    public bool closed = false;

    public void Close()
    {
        closed = true;
    }

    IEnumerator Start()
    {
        canvas_group = GetComponent<CanvasGroup>();
        for (float time = 0; time < appear_duration; time += Time.deltaTime)
        {
            canvas_group.alpha = time / appear_duration;
            yield return null;
        }
        canvas_group.alpha = 1;
        while (!closed)
            yield return null;
        
        for (float time = 0; time < disappear_duration; time += Time.deltaTime)
        {
            canvas_group.alpha = 1 - time / disappear_duration;
            yield return null;
        }
        Destroy(gameObject);
    }
}
