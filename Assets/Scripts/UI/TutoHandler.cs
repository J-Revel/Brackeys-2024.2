using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public enum TutoEvent
{
    Intro,
    Shelter,
    Storm1,
    Storm2,
}

[System.Serializable]
public struct TutoMenuConfig
{
    public TutoEvent evt;
    public GameoverMenu menu;
}

public class TutoHandler : MonoBehaviour
{
    public static TutoHandler instance;
    public TutoMenuConfig[] config;
    private bool[] menu_shown;
    
    public void Awake()
    {
        instance = this;
        menu_shown = new bool[config.Length];
    }
    
    public IEnumerator OnEvent(TutoEvent evt)
    {
        for (int i = 0; i < config.Length; i++)
        {
            if (!menu_shown[i] && config[i].evt == evt)
            {
                menu_shown[i] = true;
                GameoverMenu menu = MenuSystem.instance.OpenMenu(config[i].menu);
                while (!menu.closed)
                    yield return null;
            }
        }
    }
}
