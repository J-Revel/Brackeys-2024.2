using UnityEngine;
using UnityEngine.UI;

public class ResourceQuantityWidget: MonoBehaviour
{
    public enum DisplayMode
    {
        Flat,
        Delta
    };
    public ResourceType resource;
    public int quantity;
    public TMPro.TextMeshProUGUI text;
    public Image icon;
    public UIMainConfig config;
    
    public void Start()
    {
        text.text = quantity.ToString();
        icon.sprite = config.GetResourceIcon(resource);
    }
}