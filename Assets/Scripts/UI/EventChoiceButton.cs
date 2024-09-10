using UnityEngine;
using UnityEngine.UI;

public class EventChoiceButton : MonoBehaviour
{
    public ChoiceConfig config;
    public Button button;
    public TMPro.TextMeshProUGUI text;
    public TMPro.TextMeshProUGUI effect_description;

    public void Start()
    {
        text.text = config.description_localized.GetLocalizedString();
        string description = "";
        bool enough_resource = true;
        
        for (int i = 0; i < config.requirements.Length; i++)
        {
            if (PlayerResourceStock.instance.GetStock(config.requirements[i].resource) + config.requirements[i].delta < 0)
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
        button.interactable = enough_resource;
        
        effect_description.text = description;
    }
}
