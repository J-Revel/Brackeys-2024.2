using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ResourceType
{
    Wood, Food, Rope, 
}

[System.Serializable]
public struct ResourceStock
{
    public ResourceType resource;
    public int stock;
}

public class PlayerResourceStock : MonoBehaviour
{
    public List<ResourceStock> stocks = new List<ResourceStock>();
    public static PlayerResourceStock instance;

    public void Awake()
    {
        instance = this;
    }

    public int GetStock(ResourceType resource)
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].resource == resource)
            {
                return stocks[i].stock;
            }
        }

        return 0;
    }

    public void AddStock(ResourceType resource, int quantity)
    {
        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].resource == resource)
            {
                stocks[i] = new ResourceStock
                {
                    resource = resource,
                    stock = stocks[i].stock + quantity,
                };
                return;
            }
        }
        stocks.Add(new ResourceStock
        {
            resource = resource,
            stock = quantity,
        });
    }
}
