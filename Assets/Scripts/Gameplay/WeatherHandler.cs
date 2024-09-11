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
public class WeatherHandler : MonoBehaviour
{
    public int2 calm_phase_duration_range = new int2(2, 5);
    public int2 cloudy_phase_duration_range = new int2(2, 5);
    public int2 stormy_phase_duration_range = new int2(3, 5);
    private int3 phase_durations;
    private int current_turn;
    public float death_fade_duration = 2;
    public CanvasGroup death_canvas_group;
    public static WeatherHandler instance;

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
            Random.Range(calm_phase_duration_range.x, calm_phase_duration_range.y),
            Random.Range(cloudy_phase_duration_range.x, cloudy_phase_duration_range.y),
            Random.Range(stormy_phase_duration_range.x, stormy_phase_duration_range.y)
        );
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
        if (current_turn > phase_durations.x + phase_durations.y + phase_durations.z)
        {
            if (!GridInstance.instance.GetCellContent(PlayerController.instance.player.cell).safe)
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
            }

            current_turn = 0;
            RandomizePhaseDurations();
        }
    }
}
