using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ResourceCollectDisplay : MonoBehaviour
{
    public TMPro.TextMeshPro text;
    public ResourceType resource;
    public int quantity;
    public UIMainConfig config;
    public float fade_duration = 1;
    public float movement = 1;
    public SpriteRenderer sprite;
    public IEnumerator Start()
    {
        text.text = (quantity> 0 ? "+":"") + quantity.ToString();
        sprite.sprite = config.GetResourceIcon(resource);
        Color start_color = text.color;
        float3 start_position = transform.position;
        for (float time = 0; time < fade_duration; time += Time.deltaTime)
        {
            transform.position = start_position + new float3(0, movement * time / fade_duration, 0);
            text.color = new Color(start_color.r, start_color.g, start_color.b, 1 - time / fade_duration);
            sprite.color = new Color(1, 1, 1, 1 - time / fade_duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}
