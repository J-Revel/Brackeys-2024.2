using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ActionPopupMenu : MonoBehaviour
{
    public static ActionPopupMenu instance;
    public ResourceStock[] requirements;
    public Image image;
    public Button confirm_button;
    public Button cancel_button;
    public float appear_duration = 0.5f;
    public bool answer_selected;
    public bool confirm;
    private float3 target_scale;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        target_scale = transform.localScale;
        transform.localScale = Vector3.zero;
        confirm_button.onClick.AddListener(() =>
        {
            answer_selected = true;
            confirm = true;
        });
        
        confirm_button.onClick.AddListener(() =>
        {
            answer_selected = true;
        });
    }

    public IEnumerator ShowActionCoroutine(Sprite sprite)
    {
        image.sprite = sprite;
        answer_selected = false;
        for (float time = 0; time < appear_duration; time += Time.deltaTime)
        {
            transform.localScale = target_scale * time / appear_duration;
            yield return null;
        }

        while (!answer_selected)
            yield return null;

        for (float time = 0; time < appear_duration; time += Time.deltaTime)
        {
            transform.localScale = target_scale * (1 - time / appear_duration);
            yield return null;
        }
        
        if (confirm)
        {
            WeatherHandler.instance.SkipStorm();
        }
    }
}