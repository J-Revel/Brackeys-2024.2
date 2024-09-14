using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class EventChoiceButton : MonoBehaviour
{
    public string choice_id;
    public ChoiceConfig config;
    public Button button;
    public TMPro.TextMeshProUGUI text;
    public ResourceQuantityWidget resource_requirement_prefab;
    public Transform requirement_panel;
    public Transform result_panel;
    public GameObject lock_display;
    public AnimatedPaperWidget paper_animations;
    public System.Action selected_delegate;

    public void Start()
    {
        text.text = LocalizationSettings.StringDatabase.GetLocalizedString(choice_id);
        string description = "";
        bool enough_resource = true;
        
        requirement_panel.gameObject.SetActive(config.requirements.Length > 0);
        for (int i = 0; i < config.requirements.Length; i++)
        {
            ResourceQuantityWidget quantity_widget = Instantiate(resource_requirement_prefab, requirement_panel);
            quantity_widget.effect_type = ChoiceEffectType.Resource;
            quantity_widget.resource = config.requirements[i].resource;
            quantity_widget.quantity = config.requirements[i].delta;
            if (PlayerResourceStock.instance.GetStock(config.requirements[i].resource) < config.requirements[i].delta)
                enough_resource = false;
        }
        if (config.results.Length > 0)
        {
            for (int i = 0; i < config.results.Length; i++)
            {
                if (i > 0)
                    description += " ";
                if (config.results[i].delta > 0)
                    description += "+";
                description += config.results[i].delta;
                description += config.results[i].resource;
            }
        }
        button.onClick.AddListener(() => {selected_delegate?.Invoke();});
        lock_display.SetActive(!enough_resource);
        button.interactable = enough_resource;
        paper_animations.show_background_delegate += (ChoiceConfig config) =>
        {
            text.text = LocalizationSettings.StringDatabase.GetLocalizedString(choice_id + "_result");
            
            requirement_panel.gameObject.SetActive(false);
            for (int i = 0; i < config.results.Length; i++)
            {
                ResourceQuantityWidget quantity_widget = Instantiate(resource_requirement_prefab, result_panel);
                quantity_widget.effect_type = config.results[i].effect_type;
                quantity_widget.resource = config.results[i].resource;
                quantity_widget.quantity = config.results[i].delta;
            }
        };
    }

    public IEnumerator WaitClickCoroutine()
    {
        bool clicked = false;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            clicked = true;
        });
        while (!clicked)
            yield return null;
    }
}
