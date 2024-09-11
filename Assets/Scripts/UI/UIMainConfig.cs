using UnityEngine;

[System.Serializable]
public struct ResourceIcon
{
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
            if (resource_icon.resource == resource)
                return resource_icon.sprite;
        }

        return null;
    }
}