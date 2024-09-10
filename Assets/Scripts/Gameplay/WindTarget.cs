using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum CardinalDirection
{
    North, East, West, South,
}

public partial class GridUtils
{
    public static int2 DirectionToVector(CardinalDirection direction)
    {
            switch (direction)
            {
                case CardinalDirection.East:
                    return new int2(1, 0);
                case CardinalDirection.North:
                    return new int2(0, 1);
                case CardinalDirection.West:
                    return new int2(-1, 0);
                case CardinalDirection.South:
                    return new int2(0, -1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
    }
}
[RequireComponent(typeof(CellEntity))]
public class WindTarget : MonoBehaviour
{
    private CellEntity cell_entity;
    public float wind_speed = 3;

    public void OnEnable()
    {
        PlayerController.instance.wind_targets.Add(this);
    }

    public void OnDisable()
    {
        PlayerController.instance.wind_targets.Remove(this);
    }

    public void Start()
    {
        cell_entity = GetComponent<CellEntity>();
    }

    public IEnumerator ApplyWind(CardinalDirection wind_direction, int wind_force)
    {
        int2 direction = GridUtils.DirectionToVector(wind_direction);
        List<int2> path = new List<int2>();
        for (int i = 1; i <= wind_force; i++)
        {
            if (GridInstance.instance.IsAccessible(cell_entity.cell + direction * i))
            {
                path.Add(cell_entity.cell + direction * i);
            }
            else break;
        }
        yield return cell_entity.FollowPathCoroutine(path.ToArray(), wind_speed);
    }
}
