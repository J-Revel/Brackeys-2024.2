using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum ChoiceEffectType
{
    Resource,
    TemporaryMovement,
    PermanentMovement,
    RandomDisplacement,
}
[System.Serializable]
public struct ChoiceEffect
{
    public ChoiceEffectType effect_type;
    public ResourceType resource;
    public int delta;
}

[System.Serializable]
public struct ChoiceConfig
{
    public ChoiceEffect[] requirements;
    public ChoiceEffect[] results;
    public EventConfigAsset next_event;
}

public class EventChoiceMenu : MonoBehaviour
{
    public EventConfigList config;
    
    public TMPro.TextMeshProUGUI description_text;
    public Transform choice_panel_model;
    private Transform current_choice_panel;
    public EventChoiceButton choice_button_prefab;
    private List<EventChoiceButton> choice_buttons = new List<EventChoiceButton>();
    public CanvasGroup background_canvas_group;
    public float appear_anim_duration = 1;

    IEnumerator Start()
    {
        StartCoroutine(AppearCoroutine());
        EventConfigAsset selected_event = config.event_config[Random.Range(0, config.event_config.Length)];
        yield return ShowEventCoroutine(selected_event);
    }

    IEnumerator AppearCoroutine()
    {
        for (float time = 0; time < appear_anim_duration; time += Time.deltaTime)
        {
            background_canvas_group.alpha = time / appear_anim_duration;
            yield return null;
        }

        background_canvas_group.alpha = 1;
    }
    
    IEnumerator DisappearCoroutine()
    {
        for (float time = 0; time < appear_anim_duration; time += Time.deltaTime)
        {
            background_canvas_group.alpha = 1 - time / appear_anim_duration;
            yield return null;
        }

        background_canvas_group.alpha = 0;
        Destroy(gameObject);
    }

    public IEnumerator ShowEventCoroutine(EventConfigAsset selected_event)
    {
        List<Coroutine> coroutines = new List<Coroutine>();
        choice_buttons.Clear();
        description_text.text = LocalizationSettings.StringDatabase.GetLocalizedString(selected_event.id + "_description");
        current_choice_panel = Instantiate(choice_panel_model, choice_panel_model.parent);
        for(int i=0; i<selected_event.options.Length; i++)
        {
            ChoiceConfig choice_config = selected_event.options[i];
            EventChoiceButton choice_button = Instantiate(choice_button_prefab, current_choice_panel);
            
            coroutines.Add(StartCoroutine(choice_button.paper_animations.AppearAnimCoroutine()));
            choice_button.config = choice_config;
            choice_button.choice_id = selected_event.id + "_answer" + (i+1).ToString("00");
            ChoiceConfig active_choice_config = choice_config;
            choice_buttons.Add(choice_button);
            int option_index = i;
            choice_button.selected_delegate += () =>
            {
                foreach (ChoiceEffect result in active_choice_config.results)
                {
                    switch (result.effect_type)
                    {
                        case ChoiceEffectType.Resource:   
                            PlayerResourceStock.instance.AddStock(result.resource, result.delta);
                            break;

                        case ChoiceEffectType.TemporaryMovement:
                            PlayerController.instance.ReceiveTemporaryMovementBonus(result.delta);
                            break;
                        case ChoiceEffectType.PermanentMovement:
                            PlayerController.instance.ReceivePermanentMovementBonus(result.delta);
                            break;
                        case ChoiceEffectType.RandomDisplacement:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                StartCoroutine(ShowOutcomeCoroutine(option_index, active_choice_config, active_choice_config.next_event));
            };
        }

        foreach (Coroutine coroutine in coroutines)
            yield return coroutine;
    }

    public IEnumerator ShowOutcomeCoroutine(int option_index, ChoiceConfig choice, EventConfigAsset next_event)
    {
        var choice_panel = current_choice_panel;
        List<Coroutine> coroutines = new List<Coroutine>();
        for (int i = 0; i < choice_buttons.Count; i++)
        {
            AnimatedPaperWidget paper_widget = choice_buttons[i].paper_animations;
            if (i == option_index)
                coroutines.Add(StartCoroutine(paper_widget.FlipAnimCoroutine(choice)));
            else 
                coroutines.Add(StartCoroutine(DisappearButton(i)));
        }

        foreach (Coroutine coroutine in coroutines)
            yield return coroutine;
        yield return choice_buttons[option_index].WaitClickCoroutine();
        if (next_event != null)
        {
            StartCoroutine(choice_buttons[option_index].paper_animations.DisappearAnimCoroutine());
            
            yield return ShowEventCoroutine(next_event);
            Destroy(choice_panel.gameObject);
        }
        else
        {
            yield return choice_buttons[option_index].paper_animations.DisappearAnimCoroutine();
            yield return DisappearCoroutine();
        }
    }

    private IEnumerator DisappearButton(int index)
    {
        AnimatedPaperWidget paper_widget = choice_buttons[index].paper_animations;
        yield return paper_widget.DisappearAnimCoroutine();
    }
}

