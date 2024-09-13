using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EndingCell : MonoBehaviour
{
    public static EndingCell instance;
    public CellEntity cell_entity;
    public static int2 ending_cell { get {
        return EndingCell.instance.cell_entity.cell;
    } }

    public void Awake()
    {
        cell_entity = GetComponent<CellEntity>();
        instance = this;
    }
}
