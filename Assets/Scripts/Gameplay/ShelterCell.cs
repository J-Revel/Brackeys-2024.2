using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellEntity))]
public class ShelterCell : MonoBehaviour
{
    private CellEntity cell_entity;
    public void Start()
    {
        cell_entity = GetComponent<CellEntity>();
        GridInstance.instance.GetCellContent(cell_entity.cell).safe = true;
    }
}
