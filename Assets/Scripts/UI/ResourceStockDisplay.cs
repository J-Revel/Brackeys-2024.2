using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStockDisplay : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;
    public PlayerResource resource;

    public void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }
    
    void Update()
    {
        text.text = "" + PlayerResourceStock.instance.GetStock(resource);
    }
}
