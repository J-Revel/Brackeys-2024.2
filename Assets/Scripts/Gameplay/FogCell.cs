using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FogCell : MonoBehaviour
{
    public bool visible = false;
    public bool border = false;
    public float transition_duration = 1;
    private float time = 0;
    public SpriteRenderer sprite_renderer;
    public Color visible_color = new Color(0, 0, 0, 0);
    public Color fog_color = Color.black;
    private ParticleSystem particles;
    public ParticleSystem[] particle_systems;
    public float alpha = 0;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }
    

    void Update()
    {
        if (visible)
            time += Time.deltaTime;
        else time -= Time.deltaTime;
        time = math.clamp(time, 0, transition_duration);
        foreach (ParticleSystem particle_system in particle_systems)
        {
            particle_system.customData.SetColor(ParticleSystemCustomData.Custom1, new Color(1, 1, 1, 1 - time / transition_duration));
        }

        bool show_particles = !visible && border;
        bool show_sprite = !visible && !border;
        sprite_renderer.enabled = show_sprite;
        sprite_renderer.color = new Color(1, 1, 1, 1 - time / transition_duration);
        foreach (ParticleSystem particle_system in particle_systems)
        {
            particle_system.gameObject.SetActive(show_particles || time < transition_duration);
        }
    }
}
