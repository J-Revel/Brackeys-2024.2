using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UI;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public EventReference music_event;
    public EventReference ambience_event;
    private float music_eq_time;
    public float music_eq_transition_duration = 0.5f;

    public void Start()
    {
        EventInstance music_instance = FMODUnity.RuntimeManager.CreateInstance(music_event);
        music_instance.start();
        EventInstance ambience_instance = FMODUnity.RuntimeManager.CreateInstance(ambience_event);
        ambience_instance.start();
        music_eq_time = music_eq_transition_duration;
    }

    public void Update()
    {
        if (MenuSystem.instance.active_menu != null)
        {
            
        }
    }
}
