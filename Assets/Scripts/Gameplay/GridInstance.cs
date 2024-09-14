using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CellContent
{
    public bool accessible;
    public bool push_target;
    public List<IEnumerator> enter_coroutines = new List<IEnumerator>();
    public System.Action enter_coroutines_finished;
    
}
public class GridInstance : MonoBehaviour
{
    public static GridInstance instance;
    private GridConfig config;
    public Dictionary<int2, CellContent> cells = new Dictionary<int2, CellContent>();

    private void Awake()
    {
        config = (GridConfig)Resources.Load("GridSettings");
        instance = this;
    }

    public bool IsAccessible(int2 cell)
    {
        if (cells.TryGetValue(cell, out CellContent content))
            return content.accessible;
        return true;
    }

    public float3 CellToPos(int2 cell)
    {
        return new float3(cell.x * config.cell_size, cell.y * config.cell_size, 0);
    }
    
    public int2 PosToCell(float3 pos)
    {
        return new int2((int)math.round(pos.x / config.cell_size), (int)math.round(pos.y / config.cell_size));
    }

    public CellContent GetCellContent(int2 cell)
    {
        if (!cells.ContainsKey(cell))
        {
            CellContent new_content = new CellContent
            {
                accessible = true,
                push_target = true,
            };
            cells[cell] = new_content;
        }

        return cells[cell];
    }
}
