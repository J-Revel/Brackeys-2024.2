using System;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class MenuSystem : MonoBehaviour
    {
        public static MenuSystem instance;
        public GameObject active_menu;

        private void Awake()
        {
            instance = this;
        }

        public T OpenMenu<T>(T menu_prefab) where T: MonoBehaviour
        {
            if (active_menu != null)
            {
                Destroy(active_menu);
            }

            T new_menu = Instantiate(menu_prefab, transform);
            active_menu = new_menu.gameObject;
            return new_menu;
        }

        public void CloseMenu()
        {
            if (active_menu != null)
            {
                Destroy(active_menu);
            }
        }
    }
}