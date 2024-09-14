using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
    public Color fog_bottom_color;
    public Color fog_middle_color;
    public Color fog_top_color;
    public Color fog_sprite_color;
    public int2 duration_range;
    public int2 wind_intensity_range;
    public float wind_direction_change_ratio;
    
    public float post_exposure;
    public float contrast;
    [ColorUsage(true, true)]
    public Color color_filter;
    public float saturation;
    public float grain;
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
        wind_intensity_range = new int2(1, 2),
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

    public Material fog_bottom_mat;
    public Material fog_middle_mat;
    public Material fog_top_mat;
    public Material fog_sprite_mat;

    public float fog_transition_duration = 3;
    private WeatherPhaseConfig active_phase_config;
    private int active_phase_index = 0;
    public WindParticleSystem[] wind_particle_systems;

    public VolumeProfile post_process_profile;
    private ColorAdjustments color_adjustments;
    private FilmGrain film_grain;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        RandomizePhaseDurations();
        GameState.instance.phase_reset_delegate += () =>
        {
            ApplyWeatherPhase(calm_phase);
            current_turn = 0;
            RandomizePhaseDurations();
        };
        GameState.instance.turn_change_delegate += TurnChangeCoroutine;
        fog_bottom_mat.SetColor("_Tint", calm_phase.fog_bottom_color);
        fog_middle_mat.SetColor("_Tint", calm_phase.fog_middle_color);
        fog_top_mat.SetColor("_Tint", calm_phase.fog_top_color);
        fog_sprite_mat.SetColor("_Tint", calm_phase.fog_sprite_color);
        post_process_profile.TryGet<ColorAdjustments>(out color_adjustments);
        post_process_profile.TryGet<FilmGrain>(out film_grain);
        color_adjustments.contrast.value = calm_phase.contrast;
        color_adjustments.saturation.value = calm_phase.saturation;
        color_adjustments.postExposure.value = calm_phase.post_exposure;
        color_adjustments.colorFilter.value = calm_phase.color_filter;
        film_grain.intensity.value = calm_phase.grain;
    }

    public void RandomizePhaseDurations()
    {
        phase_durations = new int3(
            Random.Range(calm_phase.duration_range.x, calm_phase.duration_range.y),
            Random.Range(cloudy_phase.duration_range.x, cloudy_phase.duration_range.y),
            Random.Range(stormy_phase.duration_range.x, stormy_phase.duration_range.y)
        );
        CardinalDirection[] directions = new CardinalDirection[] { CardinalDirection.East, CardinalDirection.North, CardinalDirection.South, CardinalDirection.West};
        int direction_index = Random.Range(0, directions.Length);
        current_wind_direction = directions[direction_index];
        current_wind_intensity = Random.Range(calm_phase.wind_intensity_range.x, calm_phase.wind_intensity_range.y + 1);

        quaternion[] rotations = new quaternion[]
        {
                quaternion.Euler(0, 0, -math.PI / 2), quaternion.Euler(0, 0, 0),
                quaternion.Euler(0, 0, math.PI), quaternion.Euler(0, 0,  math.PI / 2)
        };
        foreach (WindParticleSystem wind_particle_system in wind_particle_systems)
        {
            wind_particle_system.transform.rotation = rotations[direction_index];
            wind_particle_system.UpdateDisplay(current_wind_intensity);
        }
    }
    

    public IEnumerator SkipStormCoroutine()
    {
        for (float time = 0; time < death_fade_duration; time += Time.deltaTime)
        {
            death_canvas_group.alpha = time / death_fade_duration;   
            yield return null;
        }
        death_canvas_group.alpha = 1;
        ApplyWeatherPhase(calm_phase);
        active_phase_index = 0;
        active_phase_config = calm_phase;

        for (float time = 0; time < death_fade_duration; time += Time.deltaTime)
        {
            death_canvas_group.alpha = 1 - time / death_fade_duration;   
            yield return null;
        }
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

    public void ApplyWeatherPhase(WeatherPhaseConfig target_phase)
    {
        fog_bottom_mat.SetColor("_Tint", target_phase.fog_bottom_color);
        fog_middle_mat.SetColor("_Tint", target_phase.fog_middle_color);
        fog_top_mat.SetColor("_Tint", target_phase.fog_top_color);
        fog_sprite_mat.SetColor("_Tint", target_phase.fog_sprite_color);
        color_adjustments.contrast.value = target_phase.contrast;
        color_adjustments.saturation.value = target_phase.saturation;
        color_adjustments.postExposure.value = target_phase.post_exposure;
        color_adjustments.colorFilter.value = target_phase.color_filter;
        film_grain.intensity.value = target_phase.grain;
    }

    IEnumerator FogTransitionCoroutine(WeatherPhaseConfig start_phase, WeatherPhaseConfig target_phase)
    {
        
        for (float time = 0; time < fog_transition_duration; time += Time.deltaTime)
        {
            float f = time / fog_transition_duration;
            fog_bottom_mat.SetColor("_Tint", Color.Lerp(start_phase.fog_bottom_color, target_phase.fog_bottom_color, f));
            fog_middle_mat.SetColor("_Tint", Color.Lerp(start_phase.fog_middle_color, target_phase.fog_middle_color, f));
            fog_top_mat.SetColor("_Tint", Color.Lerp(start_phase.fog_top_color, target_phase.fog_top_color, f));
            fog_sprite_mat.SetColor("_Tint", Color.Lerp(start_phase.fog_sprite_color, target_phase.fog_sprite_color, f));
            color_adjustments.contrast.value = math.lerp(start_phase.contrast, target_phase.contrast, f);
            color_adjustments.saturation.value = math.lerp(start_phase.saturation, target_phase.saturation, f);
            color_adjustments.postExposure.value = math.lerp(start_phase.post_exposure, target_phase.post_exposure, f);
            color_adjustments.colorFilter.value = Color.Lerp(start_phase.color_filter, target_phase.color_filter, f);
            film_grain.intensity.value = math.lerp(start_phase.grain, target_phase.grain, f);
            yield return null;
        }
        ApplyWeatherPhase(target_phase);
    }

    public void RandomizeWind()
    {
        CardinalDirection[] directions = new CardinalDirection[] { CardinalDirection.East, CardinalDirection.North, CardinalDirection.South, CardinalDirection.West};
        int direction_index = Random.Range(0, directions.Length);
        current_wind_direction = directions[direction_index];
        current_wind_intensity = Random.Range(active_phase_config.wind_intensity_range.x, active_phase_config.wind_intensity_range.y + 1);
        quaternion[] rotations = new quaternion[]
        {
            quaternion.Euler(0, 0, -math.PI / 2), quaternion.Euler(0, 0, 0),
            quaternion.Euler(0, 0, math.PI), quaternion.Euler(0, 0,  math.PI / 2)
        };
        foreach (WindParticleSystem wind_particle_system in wind_particle_systems)
        {
            wind_particle_system.transform.rotation = rotations[direction_index];
            wind_particle_system.UpdateDisplay(current_wind_intensity);
        }
    }

    IEnumerator TurnChangeCoroutine()
    {
        current_turn++;
        WeatherPhaseConfig previous_phase_config = active_phase_config;
        if (current_turn <= phase_durations.x)
        {
            active_phase_config = calm_phase;
        }
        else if (current_turn <= phase_durations.x + phase_durations.y)
        {
            active_phase_config = cloudy_phase;
            if (active_phase_index < 1)
            {
                active_phase_index = 1;
                RandomizeWind();
                StartCoroutine(FogTransitionCoroutine(calm_phase, cloudy_phase));
            }
        }
        else
        {
            active_phase_config = stormy_phase;
            if (active_phase_index < 2)
            {
                active_phase_index = 2;
                RandomizeWind();
                StartCoroutine(FogTransitionCoroutine(cloudy_phase, stormy_phase));
            }
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
            GameState.instance.phase_reset_delegate?.Invoke();
            
            for (float time = 0; time < death_fade_duration; time += Time.deltaTime)
            {
                death_canvas_group.alpha = 1 - time / death_fade_duration;   
                yield return null;
            }

            death_canvas_group.alpha = 0;

            current_turn = 0;
        }
    }
}
