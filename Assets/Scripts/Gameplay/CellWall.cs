using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CellEntity))]
public class CellWall : MonoBehaviour
{
    void Start()
    {
        int2 cell = GetComponent<CellEntity>().cell;
        CellContent cell_content = new CellContent();
        if(GridInstance.instance.cells.TryGetValue(cell, out CellContent instance_cell))
        {
            cell_content = instance_cell;
        };
        cell_content.accessible = false;
        GridInstance.instance.cells[cell] = cell_content;
    }
}
