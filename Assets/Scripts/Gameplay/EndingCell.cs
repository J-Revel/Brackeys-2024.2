using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCell : MonoBehaviour
{
    public static EndingCell instance;
    public CellEntity cell_entity;
    public static int2 ending_cell { get {
        return EndingCell.instance.cell_entity.cell;
    } }

    public string scene;

    public GameoverMenu ending_menu;
    private GameoverMenu ending_menu_instance;

    public void Awake()
    {
        cell_entity = GetComponent<CellEntity>();
        instance = this;
    }

    public void Start()
    {
        cell_entity = GetComponent<CellEntity>();
        CellContent cell_content = GridInstance.instance.GetCellContent(cell_entity.cell);
        cell_content.enter_coroutines.Add(EndingCoroutine());
    }

    public IEnumerator EndingCoroutine()
    {
        ending_menu_instance = MenuSystem.instance.OpenMenu(ending_menu);
        while (!ending_menu_instance.closed)
            yield return null;
        SceneManager.LoadScene(scene);
    }
}
