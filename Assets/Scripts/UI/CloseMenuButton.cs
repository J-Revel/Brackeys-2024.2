using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class CloseMenuButton : MonoBehaviour
{
    public void CloseMenu()
    {
        MenuSystem.instance.CloseMenu();
    }
}
