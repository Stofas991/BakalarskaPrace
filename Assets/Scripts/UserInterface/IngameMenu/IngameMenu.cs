using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenu : MonoBehaviour
{
    GameObject activeSubMenu;

    public void ActivateSubMenu(GameObject subMenu)
    {
        CloseActiveMenu(subMenu);

        subMenu.SetActive(!subMenu.activeSelf);    //activating new submenu
        activeSubMenu = subMenu;
    }

    void CloseActiveMenu(GameObject menu)
    {
        if (activeSubMenu != null && activeSubMenu != menu)
        {
            activeSubMenu.SetActive(false);
            activeSubMenu = null;
        }
    }
}
