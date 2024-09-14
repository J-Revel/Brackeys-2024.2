using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public EventReference music_event;
    public EventReference ambience_event;

    public void Start()
    {
        EventInstance music_instance = FMODUnity.RuntimeManager.CreateInstance(music_event);
        music_instance.start();
        EventInstance ambience_instance = FMODUnity.RuntimeManager.CreateInstance(ambience_event);
        ambience_instance.start();
    }
}
