using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ChoiceResult
{
    public PlayerResource resource;
    public int delta;
}

[System.Serializable]
public struct ChoiceConfig
{
    public string description;
    public ChoiceResult[] results;

}
[System.Serializable]
public struct EventConfig
{
    public string description;
    public ChoiceConfig[] options;
}
public class EventChoiceMenu : MonoBehaviour
{
    public EventConfigList config;
    public TMPro.TextMeshProUGUI description_text;
    public Transform choice_panel;
    public EventChoiceButton choice_button_prefab;
    
    void Start()
    {
        EventConfig selected_event = config.event_config[Random.Range(0, config.event_config.Length)];
        description_text.text = selected_event.description;
        foreach (ChoiceConfig choice_config in selected_event.options)
        {
            EventChoiceButton choice = Instantiate(choice_button_prefab, choice_panel);
            choice.config = choice_config;
            ChoiceConfig active_choice_config = choice_config;
            choice.button.onClick.AddListener(() =>
            {
                foreach (ChoiceResult result in active_choice_config.results)
                {
                    PlayerResourceStock.instance.AddStock(result.resource, result.delta);
                }
                Destroy(gameObject);
            });
        }
    }
}
