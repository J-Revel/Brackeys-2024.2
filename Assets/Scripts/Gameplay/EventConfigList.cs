using UnityEngine;

[CreateAssetMenu(fileName = "EventConfigList", menuName = "Event Config List", order = 0)]
public class EventConfigList : ScriptableObject
{
    public EventConfig[] event_config;
}