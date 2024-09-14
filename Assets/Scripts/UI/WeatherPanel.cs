using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class WeatherPanel : MonoBehaviour
{
    public WeatherDisplay display_prefab;
    private WeatherDisplay current_display;
    public float swap_anim_duration = 0.5f;
    public float2 max_offset = new float2(200, 0);

    public void Start()
    {
        current_display = Instantiate(display_prefab, transform);
        current_display.weather_phase = 0;
        current_display.wind_intensity = 0;
        WeatherHandler.instance.weather_change_delegate += OnWeatherChange;
        StartCoroutine(WeatherChangeCoroutine());
    }

    public void OnWeatherChange()
    {
        StartCoroutine(WeatherChangeCoroutine());
    }

    public IEnumerator WeatherChangeCoroutine()
    {
        WeatherDisplay new_display = Instantiate(display_prefab, transform);
        new_display.wind_intensity = WeatherHandler.instance.current_wind_intensity;
        new_display.wind_direction = WeatherHandler.instance.current_wind_direction;
        new_display.weather_phase = WeatherHandler.instance.current_phase;
        RectTransform rect_transform = new_display.GetComponent<RectTransform>();
        rect_transform.SetAsFirstSibling();
        for (float time = 0; time < swap_anim_duration; time += Time.deltaTime)
        {
            float f = time / swap_anim_duration;
            rect_transform.anchoredPosition = max_offset * f;
            yield return null;
        }
        rect_transform.SetAsLastSibling();
        for (float time = 0; time < swap_anim_duration; time += Time.deltaTime)
        {
            float f = 1 - time / swap_anim_duration;
            rect_transform.anchoredPosition = max_offset * f;
            yield return null;
        }
        rect_transform.anchoredPosition = Vector2.zero;
        Destroy(current_display.gameObject);
        current_display = new_display;
    }
}