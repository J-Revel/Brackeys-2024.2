using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellEntity))]
public class ResourceGainCell : MonoBehaviour
{
    public PlayerResource resource;
    public int quantity = 1;
    private CellEntity cell_entity;
    private IEnumerator enter_cell_coroutine;

    public void Start()
    {
        cell_entity = GetComponent<CellEntity>();
        enter_cell_coroutine = EnterCellCoroutine();
        cell_entity.RegisterEnterCoroutine(enter_cell_coroutine);
    }

    public void OnDestroy()
    {
        GridInstance.instance.GetCellContent(cell_entity.cell).enter_coroutines.Remove(enter_cell_coroutine);
        cell_entity.RemoveEnterCoroutine(enter_cell_coroutine);
    }

    public IEnumerator EnterCellCoroutine()
    {
        PlayerResourceStock.instance.AddStock(resource, quantity);
        Destroy(gameObject);
        yield return null;
    }
}
