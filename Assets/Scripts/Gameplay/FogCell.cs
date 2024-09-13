using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FogCell : MonoBehaviour
{
    private float time = 0;
    public ParticleSystem[] particle_systems;
    public float alpha = 0;

    void Update()
    {
        foreach (ParticleSystem particle_system in particle_systems)
        {
            particle_system.customData.SetColor(ParticleSystemCustomData.Custom1, new Color(1, 1, 1, alpha));
        }
    }
}
