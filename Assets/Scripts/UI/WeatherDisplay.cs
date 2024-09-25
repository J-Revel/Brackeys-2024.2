using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct SpriteAnimConfig
{
    public Sprite[] sprites;
    public float frame_per_second;

    public Sprite GetSprite(float time)
    {
        return sprites[Mathf.RoundToInt(frame_per_second * time) % sprites.Length];
    }
}

[Serializable]
public struct DirectionSprite
{
    public CardinalDirection direction;
    public Sprite[] sprites;
}
public class WeatherDisplay : MonoBehaviour
{
    public int weather_phase;
    public int wind_intensity;
    public CardinalDirection wind_direction;
    public Image image;
    public Image wind_direction_image;
    public SpriteAnimConfig[] weather_anims;
    public Image background_image;
    public Sprite[] background_sprites;
    public DirectionSprite[] wind_direction_sprites;
    private float time = 0;

    void Update()
    {
        time += Time.deltaTime;
        image.sprite = weather_anims[weather_phase].GetSprite(time);
        background_image.sprite = background_sprites[weather_phase];
        for (int i = 0; i < wind_direction_sprites.Length; i++)
        {
            if (wind_direction_sprites[i].direction == wind_direction)
            {
                wind_direction_image.sprite = wind_direction_sprites[i].sprites[wind_intensity];
            }
        }
    }
}
