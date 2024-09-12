using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewEventConfigAsset", menuName = "Event Config Asset", order = 0)]
public class EventConfigAsset : ScriptableObject
{
    public string id = "evt01";
    public ChoiceConfig[] options;
}
