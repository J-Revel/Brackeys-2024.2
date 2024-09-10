using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FogCell : MonoBehaviour
{
    public bool visible = false;
    public float transition_duration = 1;
    private float time = 0;
    private SpriteRenderer sprite_renderer;
    public Color visible_color = new Color(0, 0, 0, 0);
    public Color fog_color = Color.black;

    private void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }
    

    void Update()
    {
        if (visible)
            time += Time.deltaTime;
        else time -= Time.deltaTime;
        time = math.clamp(time, 0, transition_duration);
        sprite_renderer.color = Color.Lerp(fog_color, visible_color, time / transition_duration);
    }
}
