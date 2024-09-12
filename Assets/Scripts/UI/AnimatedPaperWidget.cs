using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization;
using Random = UnityEngine.Random;

public class AnimatedPaperWidget : MonoBehaviour
{
    [System.Serializable]
    public struct Octave
    {
        public float scale;
        public float wavelength;
    }
    public Octave[] x_octaves;
    public Octave[] y_octaves;
    public Octave[] angle_octaves;
    public float[] x_octaves_offsets;
    public float[] y_octaves_offsets;
    public float[] angle_octaves_offsets;
    public float appear_duration = 0.5f;
    public float flip_anim_duration = 0.5f;
    public float flip_ratio = 0;
    public TMPro.TextMeshProUGUI flip_side_text;
    private float3 default_position;
    public System.Action<ChoiceConfig> show_background_delegate;
    
    void Start()
    {
        x_octaves_offsets = new float[x_octaves.Length];
        y_octaves_offsets = new float[y_octaves.Length];
        angle_octaves_offsets = new float[angle_octaves.Length];
        for (int i = 0; i < x_octaves_offsets.Length; i++)
            x_octaves_offsets[i] = Random.value;
        for (int i = 0; i < y_octaves_offsets.Length; i++)
            y_octaves_offsets[i] = Random.value;
        for (int i = 0; i < angle_octaves_offsets.Length; i++)
            angle_octaves_offsets[i] = Random.value;
        transform.localScale = Vector3.zero;
    }

    public IEnumerator AppearAnimCoroutine()
    {
        for (float time = 0; time < appear_duration; time += Time.deltaTime)
        {
            transform.localScale = Vector3.one * time / appear_duration;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }

    void Update()
    {
        float3 position = float3.zero;
        float angle = 0;
        for(int i=0; i<x_octaves.Length; i++)
            position.x += x_octaves[i].scale * (float)math.cos(Math.PI * 2 * (Time.time * x_octaves[i].wavelength + x_octaves_offsets[i]));
        for(int i=0; i<y_octaves.Length; i++)
            position.y += y_octaves[i].scale * (float)math.cos(Math.PI * 2 * (Time.time * y_octaves[i].wavelength + y_octaves_offsets[i]));
        for(int i=0; i<angle_octaves.Length; i++)
            angle += angle_octaves[i].scale * (float)math.cos(Math.PI * 2 * (Time.time * angle_octaves[i].wavelength + angle_octaves_offsets[i]));
        transform.rotation = quaternion.Euler(0, flip_ratio * (float)Math.PI, angle);
        if(transform.parent != null)
            default_position = transform.parent.position;
        transform.position= default_position + position;
    }
    
    public IEnumerator FlipAnimCoroutine(ChoiceConfig config)
    {
        yield return null;
        bool flipped = false;
        for (float time = 0; time < flip_anim_duration; time += Time.deltaTime)
        {
            flip_ratio = time / flip_anim_duration;
            if (time > flip_anim_duration / 2)
            {
                flip_ratio -= 1;
                if (!flipped)
                {
                    show_background_delegate?.Invoke(config);
                    flipped = true;
                }
            }

            yield return null;
        }
        flip_ratio = 0;
    }

    public IEnumerator DisappearAnimCoroutine()
    {
        //transform.parent = null;
        for (float time = 0; time < appear_duration; time += Time.deltaTime)
        {
            transform.localScale = Vector3.one * (1 - time / appear_duration);
            yield return null;
        }
    }
}
