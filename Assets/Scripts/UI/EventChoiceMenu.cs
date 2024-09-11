using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public struct ResourceGain
{
    public ResourceType resource;
    public int delta;
    
}

[System.Serializable]
public struct ChoiceConfig
{
    [FormerlySerializedAs("description_localized")] public LocalizedString description;
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
    public EventChoiceMenu menu_prefab;
    private List<EventChoiceButton> choice_buttons = new List<EventChoiceButton>();
    
    void Start()
    {
        EventConfigAsset selected_event = config.event_config[Random.Range(0, config.event_config.Length)];
        ShowEvent(selected_event);
    }

    public void ShowEvent(EventConfigAsset selected_event)
    {
        foreach (EventChoiceButton button in choice_buttons)
            Destroy(button.gameObject);
        choice_buttons.Clear();
        description_text.text = selected_event.description.GetLocalizedString();
        foreach (ChoiceConfig choice_config in selected_event.options)
        {
            EventChoiceButton choice_button = Instantiate(choice_button_prefab, choice_panel);
            choice_button.config = choice_config;
            ChoiceConfig active_choice_config = choice_config;
            choice_buttons.Add(choice_button);
            choice_button.button.onClick.AddListener(() =>
            {
                foreach (ResourceGain result in active_choice_config.results)
                {
                    PlayerResourceStock.instance.AddStock(result.resource, result.delta);
                }

                if (active_choice_config.next_event != null)
                {
                    ShowEvent(active_choice_config.next_event);
                }
                else if (!active_choice_config.outcome.IsEmpty)
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
