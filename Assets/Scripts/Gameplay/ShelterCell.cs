using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CellEntity))]
public class ShelterCell : MonoBehaviour
{
    private CellEntity cell_entity;
    public ResourceStock[] cost;
    public Sprite action_icon;
    public EventReference action_sound;
    
    public void Start()
    {
        cell_entity = GetComponent<CellEntity>();
        CellContent cell_content = GridInstance.instance.GetCellContent(cell_entity.cell);
        cell_content.enter_coroutines.Add(ActionPopupMenu.instance.ShowActionCoroutine(action_icon, action_sound, cost));
        cell_content.enter_coroutines_finished += () =>
        {
            cell_content.enter_coroutines.Add(ActionPopupMenu.instance.ShowActionCoroutine(action_icon, action_sound, cost));
        };
        
    }
}
