using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CellEntity player;
    public float movement_speed = 10;
    public float range = 5;
    public GameObject target_cell_prefab;
    private List<GameObject> target_cells = new List<GameObject>();
    public CardinalDirection wind_direction;
    public int wind_strength;

    public static PlayerController instance;
    public List<WindTarget> wind_targets = new List<WindTarget>();
    public int2 current_cell;
    private bool skip;
    public PathDisplay path_display;

    public int2 checkpoint;

    public void Awake()
    {
        instance = this;
    }
    
    public IEnumerator Start()
    {
        float movement_actions = range;
        while (true)
        {
            Plane plane = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            plane.Raycast(ray, out float enter);
            float3 pos = ray.GetPoint(enter);
            int2 target_cell = new int2((int)math.round(pos.x), (int)math.round(pos.y));
            CellEntity.DijkstraMap dijkstra = player.GenerateDijkstra(movement_actions, false, Allocator.Persistent);
            int2[] targetable_cells = dijkstra.ListCells();
            
            for (int i = target_cells.Count; i < targetable_cells.Length; i++)
            {
                target_cells.Add(Instantiate(target_cell_prefab));
            }

            for (int i = targetable_cells.Length; i < target_cells.Count; i++)
            {
                target_cells[i].SetActive(false);
            }

            for (int i = 0; i < targetable_cells.Length; i++)
            {
                target_cells[i].transform.position = GridInstance.instance.CellToPos(targetable_cells[i]);
                target_cells[i].SetActive(true);
            }

            if (skip)
            {
                yield return ApplyWind(wind_direction, wind_strength);
                movement_actions = range;
                skip = false;
                yield return ActivateCell();
                yield return GameState.instance.FinishTurnCoroutine();
            }

            if (MenuSystem.instance.active_menu == null)
            {
                int2[] path = dijkstra.PathFind(target_cell, out float length);
                path_display.start_cell = dijkstra.start_cell;
                path_display.SetPath(dijkstra.start_cell, path);
                if (Input.GetMouseButtonDown(0))
                {
                    foreach (var cell in target_cells)
                    {
                        cell.SetActive(false);
                    }
                    yield return player.FollowPathCoroutine(path, movement_speed, true);
                    current_cell = GridInstance.instance.PosToCell(transform.position);
                    movement_actions -= length;
                    if (movement_actions <= 0)
                    {
                        int2 start_cell = current_cell;
                        if (wind_strength > 0)
                        {
                            yield return ApplyWind(wind_direction, wind_strength);
                        }

                        current_cell = GridInstance.instance.PosToCell(transform.position);
                        if (math.any(start_cell != current_cell))
                        {
                            yield return ActivateCell();
                        }

                        yield return GameState.instance.FinishTurnCoroutine();
                        movement_actions = range;
                    }
                }
            }

            dijkstra.data.Dispose();

            yield return null;
        }
    }

    public IEnumerator ActivateCell()
    {
        List<Coroutine> coroutines = new List<Coroutine>();
        foreach(IEnumerator enumerator in GridInstance.instance.GetCellContent(current_cell).enter_coroutines)
        {
            coroutines.Add(StartCoroutine(enumerator));
        }

        foreach (Coroutine coroutine in coroutines)
            yield return coroutine;
        
    }

    public void SkipTurn()
    {
        skip = true;
    }

    public void Update()
    {
        current_cell = GridInstance.instance.PosToCell(transform.position);
    }

    public IEnumerator ApplyWind(CardinalDirection direction, int strength)
    {
        Coroutine[] coroutines = new Coroutine[wind_targets.Count];
        for(int i=0; i<wind_targets.Count; i++)
        {
            coroutines[i] = StartCoroutine(wind_targets[i].ApplyWind(direction, strength));
        }
        foreach(Coroutine c in coroutines)
        {
            yield return c;
        }
    }

    public void TeleportToCheckpoint()
    {
        transform.position = GridInstance.instance.CellToPos(checkpoint);
        current_cell = checkpoint;
        player.cell = checkpoint;
    }
}
