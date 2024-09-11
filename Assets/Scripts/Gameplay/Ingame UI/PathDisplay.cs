using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PathDisplay : MonoBehaviour
{
    public LineRenderer line_renderer;
    public int2 start_cell;
    public int2[] path;
    private int actual_path_length;
    public float3[] previous_path;
    public float path_transition_duration = 0.3f;
    private float path_transition_time;

    public void SetPath(int2 start_cell, int2[] new_path)
    {
        if (math.any(start_cell != this.start_cell))
        {
            path = new_path;
            path_transition_time = path_transition_duration;
        }
        else
        {
            bool path_changed = new_path.Length != actual_path_length;
            if (!path_changed)
            {
                for (int i = 0; i < new_path.Length; i++)
                {
                    if(math.any(new_path[i] != path[i]))
                    {
                        path_changed = true;
                    }
                }
            }

            if (path_changed)
            {
                previous_path = new float3[math.max(new_path.Length, previous_path.Length)];
                for (int i = 0; i < previous_path.Length; i++)
                {
                    if (i < line_renderer.positionCount)
                        previous_path[i] = line_renderer.GetPosition(i);
                    else previous_path[i] = line_renderer.GetPosition(line_renderer.positionCount - 1);
                }

                path = new int2[previous_path.Length];
                actual_path_length = new_path.Length;
                for (int i = 0; i < path.Length; i++)
                {
                    if (new_path.Length > 0)
                        path[i] = new_path[math.clamp(i, 0, new_path.Length - 1)];
                    else path[i] = start_cell;
                }

                path_transition_time = 0;
            }
        }
    }
    
    public void Update()
    {
        path_transition_time += Time.deltaTime;
        line_renderer.positionCount = path.Length + 1;
        line_renderer.SetPosition(0, GridInstance.instance.CellToPos(start_cell));
        for (int i = 0; i < path.Length; i++)
        {
            float3 target_pos = GridInstance.instance.CellToPos(path[i]);
            float3 start_pos = previous_path[i];
            float f = math.saturate(path_transition_time / path_transition_duration);
            line_renderer.SetPosition(i+1, math.lerp(start_pos, target_pos, f));
        }
    }
}
