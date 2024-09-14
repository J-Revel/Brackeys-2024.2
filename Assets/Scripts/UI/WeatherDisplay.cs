using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Sprite[] weather_sprites;
    public Image background_image;
    public Sprite[] background_sprites;
    public DirectionSprite[] wind_direction_sprites;

    void Update()
    {
        image.sprite = weather_sprites[weather_phase];
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
