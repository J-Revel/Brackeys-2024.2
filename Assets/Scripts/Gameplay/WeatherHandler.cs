using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum WeatherPhase
{
    Calm,
    Cloudy,
    Stormy,
}

[System.Serializable]
public struct WeatherPhaseConfig
{
    public int2 duration_range;
    public int2 wind_intensity_range;
    public float wind_direction_change_ratio;
}
public class WeatherHandler : MonoBehaviour
{
    public WeatherPhaseConfig calm_phase = new WeatherPhaseConfig
    {
        duration_range = new int2(2, 5),
        wind_intensity_range = new int2(0, 1),
        wind_direction_change_ratio = 0.1f,
    };
    public WeatherPhaseConfig cloudy_phase = new WeatherPhaseConfig
    {
        duration_range = new int2(2, 5),
        wind_intensity_range = new int2(1, 3),
        wind_direction_change_ratio = 0.3f,
    };
    public WeatherPhaseConfig stormy_phase = new WeatherPhaseConfig
    {
        duration_range = new int2(3, 5),
        wind_intensity_range = new int2(2, 3),
        wind_direction_change_ratio = 0.7f,
    };
    private int3 phase_durations;
    private int current_turn;
    public float death_fade_duration = 2;
    public CanvasGroup death_canvas_group;
    public static WeatherHandler instance;
    public int current_wind_intensity = 0;
    public CardinalDirection current_wind_direction;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        RandomizePhaseDurations();
        GameState.instance.turn_change_delegate += TurnChangeCoroutine;
    }

    public void RandomizePhaseDurations()
    {
        phase_durations = new int3(
            Random.Range(calm_phase.duration_range.x, calm_phase.duration_range.y),
            Random.Range(cloudy_phase.duration_range.x, cloudy_phase.duration_range.y),
            Random.Range(stormy_phase.duration_range.x, stormy_phase.duration_range.y)
        );
        CardinalDirection[] directions = new CardinalDirection[] { CardinalDirection.East, CardinalDirection.North, CardinalDirection.South, CardinalDirection.West};
        current_wind_direction = directions[Random.Range(0, directions.Length)];
        current_wind_intensity = Random.Range(calm_phase.wind_intensity_range.x, calm_phase.wind_intensity_range.y + 1);
    }

    public void SkipStorm()
    {
        PlayerController.instance.checkpoint = PlayerController.instance.current_cell;
        RandomizePhaseDurations();
        current_turn = 0;
        PlayerController.instance.OnStormEnd();
    }

    public int current_phase
    {
        get
        {
            if (current_turn <= phase_durations.x)
                return 0;
            if (current_turn <= phase_durations.x + phase_durations.y)
                return 1;
            return 2;
        }
    }

    IEnumerator TurnChangeCoroutine()
    {
        current_turn++;
        WeatherPhaseConfig active_phase_config;
        if (current_turn <= phase_durations.x)
        {
            active_phase_config = calm_phase;
        }
        else if (current_turn <= phase_durations.x + phase_durations.y)
        {
            active_phase_config = cloudy_phase;
        }
        else active_phase_config = stormy_phase;

        if (Random.value < active_phase_config.wind_direction_change_ratio)
        {
            CardinalDirection[] directions = new CardinalDirection[] { CardinalDirection.East, CardinalDirection.North, CardinalDirection.South, CardinalDirection.West};
            current_wind_direction = directions[Random.Range(0, directions.Length)];
            current_wind_intensity = Random.Range(active_phase_config.wind_intensity_range.x, active_phase_config.wind_intensity_range.y + 1);
        }
        
        if (current_turn > phase_durations.x + phase_durations.y + phase_durations.z)
        {
            for (float time = 0; time < death_fade_duration; time += Time.deltaTime)
            {
                death_canvas_group.alpha = time / death_fade_duration;   
                yield return null;
            }
            death_canvas_group.alpha = 1;

            PlayerController.instance.TeleportToCheckpoint();
            
            for (float time = 0; time < death_fade_duration; time += Time.deltaTime)
            {
                death_canvas_group.alpha = 1 - time / death_fade_duration;   
                yield return null;
            }

            death_canvas_group.alpha = 0;

            current_turn = 0;
            RandomizePhaseDurations();
            PlayerController.instance.OnStormEnd();
        }
    }
}
