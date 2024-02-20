using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenu : MonoBehaviour
{
    GameObject activeSubMenu;
    public GameObject buildingSubMenu;
    public GameObject zoneSubMenu;
    public void BuildingMenu()
    {
        CloseActiveMenu(buildingSubMenu);

        buildingSubMenu.SetActive(!buildingSubMenu.activeSelf);    //activating new submenu
        activeSubMenu = buildingSubMenu;    

    }
    void CloseActiveMenu(GameObject subMenu)
    {
        if (activeSubMenu != null && activeSubMenu != subMenu)
        {
            activeSubMenu.SetActive(false);
            activeSubMenu = null;
        }
    }
}
