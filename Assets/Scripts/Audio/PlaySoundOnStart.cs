using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlaySoundOnStart : MonoBehaviour
{
    public EventReference sound_event;
    void Start()
    {
        EventInstance event_instance = FMODUnity.RuntimeManager.CreateInstance(sound_event);
        event_instance.start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
