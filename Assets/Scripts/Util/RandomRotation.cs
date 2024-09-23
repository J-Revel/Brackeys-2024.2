using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float max_rotation = 45;
    private float time = 0;
    private float rotation;
    
    void Start()
    {
        rotation = Random.Range(-max_rotation, max_rotation);
    }

    
    void Update()
    {
        time += Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, rotation * time);
    }
}
