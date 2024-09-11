using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherDisplay : MonoBehaviour
{
    public Image image;
    public Sprite[] weather_sprites;

    void Update()
    {
        image.sprite = weather_sprites[WeatherHandler.instance.current_phase];
    }
}
