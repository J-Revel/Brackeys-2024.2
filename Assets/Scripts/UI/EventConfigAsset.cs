using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewEventConfigAsset", menuName = "Event Config Asset", order = 0)]
public class EventConfigAsset : ScriptableObject
{
    [FormerlySerializedAs("description_localized")] public LocalizedString description;
    public ChoiceConfig[] options;
}
