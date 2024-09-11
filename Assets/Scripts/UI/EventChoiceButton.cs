using UnityEngine;
using UnityEngine.UI;

public class EventChoiceButton : MonoBehaviour
{
    public ChoiceConfig config;
    public Button button;
    public TMPro.TextMeshProUGUI text;
    public ResourceQuantityWidget resource_requirement_prefab;
    public Transform requirement_panel;
    public GameObject lock_display;

    public void Start()
    {
        text.text = config.description.GetLocalizedString();
        string description = "";
        bool enough_resource = true;
        
        requirement_panel.gameObject.SetActive(config.requirements.Length > 0);
        for (int i = 0; i < config.requirements.Length; i++)
        {
            ResourceQuantityWidget quantity_widget = Instantiate(resource_requirement_prefab, requirement_panel);
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
        lock_display.SetActive(!enough_resource);
        button.interactable = enough_resource;
    }
}
