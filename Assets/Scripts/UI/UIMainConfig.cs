using UnityEngine;

[System.Serializable]
public struct ResourceIcon
{
    public ChoiceEffectType effect_type;
    public ResourceType resource;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "UI Main Config", menuName = "UI Main Config", order = 0)]
public class UIMainConfig : ScriptableObject
{
    public ResourceIcon[] resource_icons;

    public Sprite GetResourceIcon(ResourceType resource)
    {
        foreach (ResourceIcon resource_icon in resource_icons)
        {
            if (resource_icon.effect_type == ChoiceEffectType.Resource && resource_icon.resource == resource)
                return resource_icon.sprite;
        }

        return null;
    }

    public Sprite GetEffectIcon(ChoiceEffectType effect, ResourceType resource)
    {
        if (effect == ChoiceEffectType.Resource)
            return GetResourceIcon(resource);
        foreach (ResourceIcon resource_icon in resource_icons)
        {
            if (resource_icon.effect_type == effect)
                return resource_icon.sprite;
        }

        return null;
    }
}