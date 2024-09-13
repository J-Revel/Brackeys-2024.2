using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Localization.Plugins.XLIFF.V12;
#endif
using UnityEngine;

[ExecuteAlways]
public class CellEntity : MonoBehaviour
{
    public int2 cell;
    private GridConfig grid_config;
    public List<IEnumerator> enter_coroutines = new List<IEnumerator>();
    public EventReference move_audio_event;

    private void Start()
    {
        grid_config = (GridConfig)Resources.Load("GridSettings");
        transform.position = new float3(cell.x * grid_config.cell_size, cell.y * grid_config.cell_size, 0);
    }
    
    #if UNITY_EDITOR
    public void Update()
    {
        #if UNITY_EDITOR
        if (!Application.IsPlaying(this))
        {
            if (grid_config == null)
            {
                grid_config = (GridConfig)Resources.Load("GridSettings");
            }

            SerializedObject serialized_object = new SerializedObject(this);
            serialized_object.FindProperty("cell").FindPropertyRelative("x").intValue = (int)(transform.position.x / grid_config.cell_size);
            serialized_object.FindProperty("cell").FindPropertyRelative("y").intValue = (int)(transform.position.y / grid_config.cell_size);
            serialized_object.ApplyModifiedPropertiesWithoutUndo();

        }
        #endif
    }
    #endif

    public void MoveTo(int2 target_cell, float speed, bool activate_cells)
    {
        StartCoroutine(MoveToCoroutine(target_cell, speed, activate_cells));
    }
    
    public void FollowPath(int2[] path, float speed, bool activate_cells)
    {
        StartCoroutine(FollowPathCoroutine(path, speed, activate_cells));
    }
    
    public IEnumerator FollowPathCoroutine(int2[] path, float speed, bool activate_cells)
    {
        for (int i = 0; i < path.Length; i++)
        {
            yield return MoveToCoroutine(path[i], speed, activate_cells);
        }
    }

    public IEnumerator ActivateCell(int2 cell)
    {
        List<Coroutine> coroutines = new List<Coroutine>();
        CellContent cell_content = GridInstance.instance.GetCellContent(cell);
        foreach(IEnumerator enumerator in cell_content.enter_coroutines)
        {
            coroutines.Add(StartCoroutine(enumerator));
        }

        foreach (Coroutine coroutine in coroutines)
            yield return coroutine;
        cell_content.enter_coroutines_finished?.Invoke();
    }
    
    public IEnumerator MoveToCoroutine(int2 target_cell, float speed, bool activate_cells)
    {
        if (!move_audio_event.IsNull)
        {
            EventInstance move_event_state = FMODUnity.RuntimeManager.CreateInstance(move_audio_event);
            move_event_state.start();
        }
        CellContent start_cell_content = GridInstance.instance.GetCellContent(cell);
        float distance = math.distance((float2)target_cell, (float2)cell);
        float2 start_pos = (float2)cell;
        float2 target_pos = (float2)target_cell;
        int2 start_cell = cell;
        cell = target_cell;
        float duration = distance / speed;
        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            float2 display_pos = math.lerp(start_pos, target_pos, time / duration);
            transform.position = new float3(display_pos.x, display_pos.y, 0);
            yield return null;
        }

        if (activate_cells)
        {
            List<Coroutine> coroutines = new List<Coroutine>();
            foreach(IEnumerator enumerator in GridInstance.instance.GetCellContent(start_cell).leave_coroutines)
            {
                coroutines.Add(StartCoroutine(enumerator));
            }
            coroutines.Add(StartCoroutine(ActivateCell(target_cell)));

            foreach (Coroutine coroutine in coroutines)
                yield return coroutine;
        }
        CellContent target_cell_content = GridInstance.instance.GetCellContent(target_cell);
        foreach (IEnumerator coroutine in enter_coroutines)
        {
            target_cell_content.enter_coroutines.Add(coroutine);
        }
    }

    public struct CellDistance
    {
        public int2 cell;
        public float distance;
    }

    public struct DijkstraMapElement
    {
        public float distance;
        public int2 previous_cell;
    }


    public struct DijkstraMap
    {
        public DijkstraMap(int2 start_cell, Allocator allocator)
        {
            this.start_cell = start_cell;
            data = new Dictionary<int2, DijkstraMapElement>();
        }
        public int2 start_cell;
        public Dictionary<int2, DijkstraMapElement> data;
        public int2[] PathFind(int2 target_cell, out float length)
        {
            length = 0;
            NativeList<int2> path_reversed = new NativeList<int2>(Allocator.Temp);
            int2 cursor = target_cell;
            while (data.ContainsKey(cursor) && math.any(cursor != start_cell))
            {
                path_reversed.Add(cursor);
                cursor = data[cursor].previous_cell;
            }

            int2[] result = new int2[path_reversed.Length];
            cursor = start_cell;
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = path_reversed[path_reversed.Length - i - 1];
                length += math.abs(cursor.x - result[i].x) + math.abs(cursor.y - result[i].y);
                cursor = result[i];
            }

            return result;
        }

        public int2[] ListCells()
        {
            int2[] result = new int2[data.Count];
            int cursor = 0;
            foreach (KeyValuePair<int2, DijkstraMapElement> data_pair in data)
            {
                result[cursor] = data_pair.Key;
                cursor++;
            }

            return result;
        }
    }
    
    public DijkstraMap GenerateDijkstra(float range, bool flying, Allocator allocator)
    {
        NativeHashMap<int2, float> distances = new NativeHashMap<int2, float>(1024, Allocator.Temp);
        NativeList<CellDistance> to_handle = new NativeList<CellDistance>(Allocator.Temp);
        DijkstraMap result = new DijkstraMap(cell, allocator);
        result.data.Add(cell, new DijkstraMapElement
        {
            distance = 0,
        });
        distances[cell] = 0;
        to_handle.Add(new CellDistance{
            cell=cell, 
            distance=0
        });
        int cursor = 0;
        while (to_handle.Length > 0)
        {
            CellDistance next_to_handle = to_handle[^1];
            to_handle.RemoveAt(to_handle.Length - 1);
            int2 current_cell = next_to_handle.cell;
            float current_length = next_to_handle.distance;
            int2[] neighbour_cells = new int2[]
            {
                current_cell + new int2(1, 0),
                current_cell + new int2(-1, 0),
                current_cell + new int2(0, 1),
                current_cell + new int2(0, -1),
            };

            if (current_length + 1 <= range)
            {
                foreach (int2 neighbour in neighbour_cells)
                {
                    if (GridInstance.instance.IsAccessible(neighbour))
                    {
                        if (!distances.ContainsKey(neighbour))
                        {
                            result.data[neighbour] = new DijkstraMapElement
                            {
                                distance = current_length + 1,
                                previous_cell = current_cell,
                            };
                            distances[neighbour] = current_length + 1;
                            to_handle.Add(new CellDistance
                            {
                                cell = neighbour, 
                                distance = current_length + 1
                            });
                        }
                        else if (distances[neighbour] > current_length + 1)
                        {
                            result.data[neighbour] = new DijkstraMapElement
                            {
                                distance = current_length + 1,
                                previous_cell = current_cell,
                            };
                            distances[neighbour] = current_length + 1;
                            to_handle.Add(new CellDistance
                            {
                                cell = neighbour,
                                distance = current_length + 1
                            });
                        }
                    }
                }
            }
            cursor++;
        }

        return result;
    }

    public void RegisterEnterCoroutine(IEnumerator coroutine)
    {
        enter_coroutines.Add(coroutine);
        GridInstance.instance.GetCellContent(cell).enter_coroutines.Add(coroutine);
    }

    public void RemoveEnterCoroutine(IEnumerator coroutine)
    {
        enter_coroutines.Remove(coroutine);
        GridInstance.instance.GetCellContent(cell).enter_coroutines.Remove(coroutine);
    }
}
