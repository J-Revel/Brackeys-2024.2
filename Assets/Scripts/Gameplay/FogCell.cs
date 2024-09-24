using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FogCell : MonoBehaviour
{
    private float time = 0;
    public ParticleSystem[] particle_systems;
    public SpriteRenderer[] sprite_renderers;
    public float alpha = 0;

    void Start()
    {
        Color color = new Color(1, 1, 1, alpha * alpha * alpha);
        foreach (ParticleSystem particle_system in particle_systems)
            particle_system.customData.SetColor(ParticleSystemCustomData.Custom1, new Color(1, 1, 1, alpha * alpha * alpha));
        foreach (SpriteRenderer sprite_renderer in sprite_renderers)
            sprite_renderer.color = color;
        GetComponent<ParticleSystem>().Play();
    }
    void Update()
    {
        Color color = new Color(1, 1, 1, alpha * alpha * alpha);
        foreach (ParticleSystem particle_system in particle_systems)
            particle_system.customData.SetColor(ParticleSystemCustomData.Custom1, color);

        foreach (SpriteRenderer sprite_renderer in sprite_renderers)
            sprite_renderer.color = color;
    }
}
