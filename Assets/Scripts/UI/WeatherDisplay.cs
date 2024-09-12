using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct DirectionSprite
{
    public CardinalDirection direction;
    public Sprite sprite;
}
public class WeatherDisplay : MonoBehaviour
{
    public Image image;
    public Image wind_direction_image;
    public TMPro.TextMeshProUGUI wind_intensity_text;
    public Sprite[] weather_sprites;
    public DirectionSprite[] wind_direction_sprites;

    void Update()
    {
        image.sprite = weather_sprites[WeatherHandler.instance.current_phase];
        wind_intensity_text.text = "x" + WeatherHandler.instance.current_wind_intensity;
        for (int i = 0; i < wind_direction_sprites.Length; i++)
        {
            if (wind_direction_sprites[i].direction == WeatherHandler.instance.current_wind_direction)
            {
                wind_direction_image.sprite = wind_direction_sprites[i].sprite;
            }
        }
    }
}
