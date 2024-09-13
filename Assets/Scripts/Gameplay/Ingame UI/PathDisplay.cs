using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class PathDisplay : MonoBehaviour
{
    public LineRenderer line_renderer;
    public int2 start_cell;
    public int2[] path;
    private int actual_path_length;
    public float3[] previous_path;
    public float path_transition_duration = 0.3f;
    private float path_transition_time;
    private uint random_seed;
    public float2 random_offset_scale = new float2(0.2f, 0.5f);
    public GameObject[] point_prefabs;
    public List<GameObject> spawned_points = new List<GameObject>();

    private void Start()
    {
        random_seed = (uint)DateTime.Now.Ticks;
    }
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
                foreach(GameObject to_remove in spawned_points)
                    Destroy(to_remove);
                spawned_points.Clear();
                random_seed = (uint)DateTime.Now.Ticks;
                previous_path = new float3[math.max(new_path.Length, previous_path.Length)];
                for (int i = 0; i < previous_path.Length; i++)
                {
                    if (i < line_renderer.positionCount)
                        previous_path[i] = line_renderer.GetPosition(i);
                    else previous_path[i] = line_renderer.GetPosition(line_renderer.positionCount - 1);
                }

                path = new int2[new_path.Length];
                actual_path_length = new_path.Length;
                for (int i = 0; i < path.Length; i++)
                {
                    if (new_path.Length > 0)
                        path[i] = new_path[math.clamp(i, 0, new_path.Length - 1)];
                    else path[i] = start_cell;
                }

                path_transition_time = 0;
                for (int i = 0; i < path.Length; i++)
                    spawned_points.Add(Instantiate(point_prefabs[UnityEngine.Random.Range(0, point_prefabs.Length)]));
            }
        }
    }
    
    public void Update()
    {
        path_transition_time += Time.deltaTime;
        line_renderer.positionCount = path.Length + 1;
        Random random = new Random(random_seed);
        line_renderer.SetPosition(0, GridInstance.instance.CellToPos(start_cell));
        for (int i = 0; i < path.Length; i++)
        {
            float2 random_offset = random.NextFloat2();
            float3 target_pos = GridInstance.instance.CellToPos(path[i]);
            float offset = math.lerp(random_offset_scale.x, random_offset_scale.y, random_offset.y);
            if (i < path.Length - 1)
            {
                target_pos.x += offset * math.cos(random_offset.x * math.PI * 2);
                target_pos.y += offset * math.sin(random_offset.x * math.PI * 2);
            }
            line_renderer.SetPosition(i+1, target_pos);
            spawned_points[i].transform.position = target_pos;
        }
    }
}
