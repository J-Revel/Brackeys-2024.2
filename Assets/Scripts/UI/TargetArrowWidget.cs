using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TargetArrowWidget : MonoBehaviour
{
    public float max_offset = 10;
    public float display_offset = 0;
    public bool rotate = true;
    void Start()
    {
        
    }
    void Update()
    {
        float2 offset = (GridInstance.instance.CellToPos(EndingCell.ending_cell) - (float3)PlayerController.instance.transform.position).xy;
        float offset_length = math.clamp(math.length(offset), 0, max_offset) + display_offset;

        transform.position = (float3)PlayerController.instance.transform.position + math.normalize(new float3(offset.x, offset.y, 0)) * offset_length;
        if(rotate)
            transform.rotation = quaternion.Euler(0, 0, math.atan2(offset.y, offset.x));
    }
}
