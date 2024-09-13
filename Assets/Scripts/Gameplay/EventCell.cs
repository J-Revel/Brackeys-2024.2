using System.Collections;
using UI;
using UnityEngine;

[RequireComponent(typeof(CellEntity))]
public class EventCell : MonoBehaviour
{
    public EventConfigList config_list;
    public EventChoiceMenu choice_menu_prefab;
    private CellEntity cell_entity;
    private IEnumerator show_event_coroutine;

    public void Start()
    {
        cell_entity = GetComponent<CellEntity>();
        show_event_coroutine = ShowEventCoroutine();
        cell_entity.RegisterEnterCoroutine(show_event_coroutine);
    }

    public void OnDestroy()
    {
        CellContent cell_content = GridInstance.instance.GetCellContent(cell_entity.cell);
        cell_entity.RemoveEnterCoroutine(show_event_coroutine);
    }

    public IEnumerator ShowEventCoroutine()
    {
        EventChoiceMenu choice_menu = MenuSystem.instance.OpenMenu(choice_menu_prefab);
        choice_menu.config = config_list;
        while (choice_menu != null)
            yield return null;
        Destroy(gameObject);
    }
}