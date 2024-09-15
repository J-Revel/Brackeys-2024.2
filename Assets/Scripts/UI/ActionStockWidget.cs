using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public class ActionStockWidget : MonoBehaviour
{
    public Image[] star_images;
    public Image[] line_images;
    public Sprite[] full_star_sprites;
    public Sprite[] half_star_sprites;
    public float transition_durations = 0.5f;
    public float[] animated_scales;
    private uint random_seed;
    void Start()
    {
        random_seed = (uint)DateTime.Now.Ticks;
        animated_scales = new float[star_images.Length];
        for (int i = 0; i < star_images.Length; i++)
        {
            Random rand = new Random(random_seed);
            star_images[i].sprite = full_star_sprites[rand.NextInt(full_star_sprites.Length)];
        }
    }

    void Update()
    {
        Random rand = new Random(random_seed);
        for (int i = 0; i < animated_scales.Length; i++)
        {
            float direction = PlayerController.instance.movement_actions > i ? 1 : -1;
            animated_scales[i] = math.clamp(animated_scales[i] + Time.deltaTime / transition_durations * direction, 0, 1);
            star_images[i].transform.localScale = Vector3.one * animated_scales[i];
            if (i < PlayerController.instance.range)
                star_images[i].sprite = full_star_sprites[rand.NextInt(full_star_sprites.Length)];
            else
                star_images[i].sprite = half_star_sprites[rand.NextInt(half_star_sprites.Length)];
            if(i > 0)
                line_images[i-1].transform.localScale = new float3(1, animated_scales[i], 1);
        }
    }
}
