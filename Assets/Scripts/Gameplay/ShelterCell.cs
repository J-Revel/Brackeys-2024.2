using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CellEntity))]
public class ShelterCell : MonoBehaviour
{
    private CellEntity cell_entity;
    public Transform shelter_icon_prefab;
    public float appear_duration = 0.3f;
    public float3 icon_offset = new float3(0, 1, 0);
    public bool hide = false;
    public void Start()
    {
        cell_entity = GetComponent<CellEntity>();
        GridInstance.instance.GetCellContent(cell_entity.cell).enter_coroutines.Add(EnterCoroutine());
        GridInstance.instance.GetCellContent(cell_entity.cell).leave_coroutines.Add(LeaveCoroutine());
    }

    public IEnumerator EnterCoroutine()
    {
        yield return null;
        StartCoroutine(ShowIconCoroutine());
    }

    public IEnumerator LeaveCoroutine()
    {
        yield return null;
        hide = true;
    }
    public IEnumerator ShowIconCoroutine()
    {
        Transform shelter_icon = Instantiate(shelter_icon_prefab, transform.position, quaternion.identity);
        for (float time = 0; time < appear_duration; time += Time.deltaTime)
        {
            shelter_icon.transform.position = (float3)transform.position + icon_offset * time / appear_duration;
            
            yield return null;
        }
        shelter_icon.transform.position = (float3)transform.position + icon_offset;
        while (!hide)
        {
            yield return null;
        }

        hide = false;

        for (float time = 0; time < appear_duration; time += Time.deltaTime)
        {
            shelter_icon.transform.position = (float3)transform.position + icon_offset * time / appear_duration;
            yield return null;
        }
        shelter_icon.transform.position = (float3)transform.position + icon_offset;
        Destroy(shelter_icon);
    }
}
