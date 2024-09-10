using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public struct ResourceGain
{
    public PlayerResource resource;
    public int delta;
    
}

[System.Serializable]
public struct ChoiceConfig
{
    public LocalizedString description_localized;
    public ResourceGain[] requirements;
    public ResourceGain[] results;
    public int temporary_movement_bonus;
    public int permanent_movement_bonus;
    public int random_movement;
    public LocalizedString outcome;
    public EventConfigAsset next_event;
}

[System.Serializable]
public struct EventConfig
{
    public LocalizedString description_localized;
    public ChoiceConfig[] options;
}

public class EventChoiceMenu : MonoBehaviour
{
    public EventConfigList config;
    public TMPro.TextMeshProUGUI description_text;
    public Transform choice_panel;
    public EventChoiceButton choice_button_prefab;
    public GameObject outcome_panel;
    public TMPro.TextMeshProUGUI outcome_text;
    
    void Start()
    {
        EventConfigAsset selected_event = config.event_config[Random.Range(0, config.event_config.Length)];
        description_text.text = selected_event.description.GetLocalizedString();
        foreach (ChoiceConfig choice_config in selected_event.options)
        {
            EventChoiceButton choice = Instantiate(choice_button_prefab, choice_panel);
            choice.config = choice_config;
            ChoiceConfig active_choice_config = choice_config;
            choice.button.onClick.AddListener(() =>
            {
                foreach (ResourceGain result in active_choice_config.results)
                {
                    PlayerResourceStock.instance.AddStock(result.resource, result.delta);
                }

                if (!active_choice_config.outcome.IsEmpty)
                {
                    outcome_text.text = active_choice_config.outcome.GetLocalizedString();
                    outcome_panel.SetActive(true);
                }
                else
                    Destroy(gameObject);
            });
        }
    }
}
