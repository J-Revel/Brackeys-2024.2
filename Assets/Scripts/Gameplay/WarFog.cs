using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class WarFog : MonoBehaviour
{
    public FogCell cell_prefab;
    public SpriteRenderer sprite_cell_prefab;
    public List<int2> used_cell_coords = new List<int2>();
    private List<SpriteRenderer> temporary_sprite_cells = new List<SpriteRenderer>();
    public List<FogCell> fog_cells = new List<FogCell>();
    public int fog_cell_size = 10;
    public float vision_range = 5;
    public float transition_duration = 1;

    public enum CellState
    {
        Outside,
        Cell,
        Border,
        NoFog,
    };

    public enum CellCommandType
    {
        Sprite,
        Particle,
    }

    public struct CellCommandAction
    {
        public CellCommandType command_type;
        public bool spawn;
        public bool destroy;
        public float start_alpha, end_alpha;
    }

    public class CellCommandState
    {
        public CellCommandAction action;
        public SpriteRenderer sprite_renderer;
        public FogCell fog_cell;
        public float ratio;
        public int2 cell;
        
        public GameObject StartAction(float3 position, GameObject existing_object, SpriteRenderer sprite_prefab, FogCell particle_prefab)
        {
            GameObject result = null;
            if (action.spawn)
            {
                switch (action.command_type)
                {
                    case CellCommandType.Particle:
                        fog_cell = Instantiate(particle_prefab, position, Quaternion.identity);
                        fog_cell.gameObject.name = "Particle Cell " + cell;
                        result = fog_cell.gameObject;
                        break;
                    case CellCommandType.Sprite:
                        sprite_renderer = Instantiate(sprite_prefab, position, Quaternion.identity);
                        sprite_renderer.gameObject.name = "Sprite Cell " + cell;
                        result = sprite_renderer.gameObject;
                        break;
                }
            }
            else if(existing_object != null)
            {
                fog_cell = existing_object.GetComponent<FogCell>();
                sprite_renderer = existing_object.GetComponent<SpriteRenderer>();
                result = existing_object;
            }
            if(sprite_renderer != null)
                sprite_renderer.color = new Color(1, 1, 1, action.start_alpha);
            if (fog_cell != null)
                fog_cell.alpha = action.start_alpha;
            return result;
        }
        
        public void UpdateAction()
        {
            float alpha = math.lerp(action.start_alpha, action.end_alpha, ratio);
            if(sprite_renderer != null)
                sprite_renderer.color = new Color(1, 1, 1, alpha * alpha);
            if (fog_cell != null)
                fog_cell.alpha = alpha;
        }

        public void FinishAction()
        {
            if (action.destroy)
            {
                if(sprite_renderer != null)
                    Destroy(sprite_renderer.gameObject);
                if(fog_cell != null)
                    Destroy(fog_cell.gameObject);
            }
        }
    }

    public struct CellDisplayData
    {
        public CellState state;
        public GameObject game_object;

        public CellCommandAction[] ListTransition(int2 cell, CellState new_state)
        {
            if (state == new_state)
                return Array.Empty<CellCommandAction>();
            switch (state)
            {
                case CellState.Outside:
                    switch (new_state)
                    {
                        case CellState.Border:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Sprite, start_alpha = 1, end_alpha = 0, spawn = true, destroy = true,
                                },
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Particle, start_alpha = 1, end_alpha = 1, destroy = false,
                                    spawn = true
                                },
                            };
                        case CellState.Cell:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Sprite, start_alpha = 1, end_alpha = 1, spawn = true, destroy = false,
                                },
                            };
                        case CellState.NoFog:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Sprite, start_alpha = 1, end_alpha = 0, spawn = true, destroy = true,
                                },
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Particle, start_alpha = 1, end_alpha = 0, spawn = true, destroy = true,
                                },
                            };
                    }
                    break;
                case CellState.Cell:
                    switch (new_state)
                    {
                        case CellState.Outside:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Sprite, start_alpha = 0, end_alpha = 0, spawn = false, destroy = true,
                                },
                            };
                        case CellState.Border:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Sprite, start_alpha = 1, end_alpha = 0, spawn = false, destroy = true,
                                },
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Particle, start_alpha = 1, end_alpha = 1, spawn = true, destroy = false,
                                },
                            };
                        case CellState.NoFog:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Sprite, start_alpha = 1, end_alpha = 0, spawn = false, destroy = true,
                                },
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Particle, start_alpha = 1, end_alpha = 0, spawn = true, destroy = true,
                                },
                            };
                    }
                    break;
                case CellState.Border:
                    switch (new_state)
                    {
                        case CellState.Outside:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Sprite, start_alpha = 0, end_alpha = 1, spawn = true,
                                    destroy = true,
                                },
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Particle, start_alpha = 1, end_alpha = 1,
                                    spawn = false, destroy = true,
                                },
                            };
                        case CellState.Cell:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Sprite, start_alpha = 0, end_alpha = 1, spawn = true,
                                    destroy = false,
                                },
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Particle, start_alpha = 1, end_alpha = 1,
                                    spawn = false, destroy = true,
                                },
                            };
                        case CellState.NoFog:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Particle, start_alpha = 1, end_alpha = 0,
                                    spawn = false, destroy = true,
                                },
                            };
                    }

                    break;
                case CellState.NoFog:
                    switch (new_state)
                    {
                        case CellState.Outside:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Sprite, start_alpha = 0, end_alpha = 1, spawn = true, destroy = true,
                                },
                            };
                        case CellState.Cell:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Sprite, start_alpha = 0, end_alpha = 1, spawn = true, destroy = false,
                                },
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Particle, start_alpha = 0, end_alpha = 1, spawn = true, destroy = true,
                                },
                            };
                        case CellState.Border:
                            return new CellCommandAction[]
                            {
                                new CellCommandAction
                                {
                                    command_type = CellCommandType.Particle, start_alpha = 0, end_alpha = 1, spawn = true, destroy = false,
                                },
                            };
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new CellCommandAction[] { };
        }
    }

    public Dictionary<int2, CellDisplayData> cell_states = new Dictionary<int2, CellDisplayData>();

    public void Start()
    {
        GameState.instance.turn_change_delegate += UpdateFog;
        GameState.instance.phase_reset_delegate += () => StartCoroutine(UpdateFog());
        StartCoroutine(UpdateFog());
    }
    public IEnumerator UpdateFog()
    {
        int2 player_cell = PlayerController.instance.current_cell;

        foreach (SpriteRenderer to_remove in temporary_sprite_cells)
        {
            Destroy(to_remove.gameObject);
        }
        temporary_sprite_cells.Clear();

        List<CellCommandState> cell_command_states = new List<CellCommandState>();
        for (int i = -fog_cell_size; i <= fog_cell_size; i++)
        {
            for (int j = -fog_cell_size; j <= fog_cell_size; j++)
            {
                CellState new_state = CellState.NoFog;
                int2 cell = player_cell + new int2(i, j);
                GameObject visible_element = cell_states.ContainsKey(cell) ? cell_states[cell].game_object: null;
                if(math.lengthsq(new int2(i, j)) > vision_range * vision_range)
                {
                    int2[] neighbour_cells = new int2[]
                    {
                        cell + new int2(1, 0),
                        cell + new int2(-1, 0),
                        cell + new int2(1, 1),
                        cell + new int2(-1, 1),
                        cell + new int2(0, 1),
                        cell + new int2(1, -1),
                        cell + new int2(-1, -1),
                        cell + new int2(0, -1),
                    };
                    bool is_border = false;
                    foreach (int2 neighbour in neighbour_cells)
                    {
                        if (math.lengthsq(neighbour - player_cell) <= vision_range * vision_range)
                        {
                            is_border = true;
                            break;
                        }
                    }

                    new_state = is_border ? CellState.Border : CellState.Cell;

                }
                CellDisplayData previous_display_data = new CellDisplayData
                    { state = CellState.Outside, game_object = null };
                if (cell_states.TryGetValue(cell, out CellDisplayData state))
                {
                    previous_display_data = state;
                }
                CellCommandAction[] transition = previous_display_data.ListTransition(cell, new_state);
                foreach (CellCommandAction action in transition)
                {
                    CellCommandState command_state = new CellCommandState
                    {
                        action = action,
                        ratio = 0,
                        cell = cell,
                    };
                    GameObject target_element = command_state.StartAction(GridInstance.instance.CellToPos(cell), previous_display_data.game_object, sprite_cell_prefab, cell_prefab);
                    if (!command_state.action.destroy)
                        visible_element = target_element;
                    cell_command_states.Add(command_state);
                }

                cell_states[cell] = new CellDisplayData { state = new_state, game_object = visible_element };
            }
        }
        SpriteRenderer right_big_tile = Instantiate(sprite_cell_prefab, GridInstance.instance.CellToPos(player_cell + new int2(fog_cell_size * 2, 0)), Quaternion.identity);
        right_big_tile.transform.localScale = Vector3.one * (fog_cell_size * 2);
        temporary_sprite_cells.Add(right_big_tile);
        SpriteRenderer left_big_tile = Instantiate(sprite_cell_prefab, GridInstance.instance.CellToPos(player_cell - new int2(fog_cell_size * 2, 0)), Quaternion.identity);
        left_big_tile.transform.localScale = Vector3.one * (fog_cell_size * 2);
        temporary_sprite_cells.Add(left_big_tile);
        SpriteRenderer top_big_tile = Instantiate(sprite_cell_prefab, GridInstance.instance.CellToPos(player_cell + new int2(0, fog_cell_size * 2)), Quaternion.identity);
        top_big_tile.transform.localScale = Vector3.one * (fog_cell_size * 2);
        temporary_sprite_cells.Add(top_big_tile);
        SpriteRenderer bottom_big_tile = Instantiate(sprite_cell_prefab, GridInstance.instance.CellToPos(player_cell - new int2(0, fog_cell_size * 2)), Quaternion.identity);
        bottom_big_tile.transform.localScale = Vector3.one * (fog_cell_size * 2);
        temporary_sprite_cells.Add(bottom_big_tile);

        StartCoroutine(PlayTransition(cell_command_states));
        yield return null;
    }

    public IEnumerator PlayTransition(List<CellCommandState> cell_command_states)
    {
        for (float time = 0; time < transition_duration; time += Time.deltaTime)
        {
            foreach(CellCommandState command_state in cell_command_states)
            {
                command_state.ratio = time / transition_duration;
                command_state.UpdateAction();
            }
            yield return null;
        }
        foreach(CellCommandState command_state in cell_command_states)
        {
            command_state.ratio = 1;
            command_state.FinishAction();
        }
        
    }
}
