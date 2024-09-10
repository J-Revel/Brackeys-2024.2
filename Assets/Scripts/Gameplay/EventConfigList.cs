using UnityEngine;

[CreateAssetMenu(fileName = "EventConfigList", menuName = "Event Config List", order = 0)]
public class EventConfigList : ScriptableObject
{
    public EventConfigAsset[] event_config;
}