using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class WarFog : MonoBehaviour
{
    public FogCell cell_prefab;
    public Dictionary<int2, FogCell> warfog_cells = new Dictionary<int2, FogCell>();
    public int fog_cell_size = 10;
    public int vision_range = 5;

    public void Update()
    {
        int2 player_cell = PlayerController.instance.current_cell;
        for (int i = -fog_cell_size; i < fog_cell_size; i++)
        {
            for (int j = -fog_cell_size; j < fog_cell_size; j++)
            {
                int2 cursor = new int2(player_cell.x + i, player_cell.y + j);
                if (!warfog_cells.ContainsKey(cursor))
                {
                    warfog_cells[cursor] = Instantiate(cell_prefab, GridInstance.instance.CellToPos(cursor), Quaternion.identity);
                    warfog_cells[cursor].visible = false;
                }

                warfog_cells[cursor].visible = math.abs(i) + math.abs(j) < vision_range;
                int2[] neighbour_cells = new int2[]
                {
                    cursor + new int2(1, 0),
                    cursor + new int2(-1, 0),
                    cursor + new int2(1, 1),
                    cursor + new int2(-1, 1),
                    cursor + new int2(0, 1),
                    cursor + new int2(1, -1),
                    cursor + new int2(-1, -1),
                    cursor + new int2(0, -1),
                };
                bool is_border = false;
                foreach (int2 neighbour_cell in neighbour_cells)
                {
                    if (warfog_cells.ContainsKey(neighbour_cell) && warfog_cells[neighbour_cell].visible)
                    {
                        is_border = true;
                        break;
                    }
                }

                warfog_cells[cursor].border = is_border;
            }
        }
    }
}
