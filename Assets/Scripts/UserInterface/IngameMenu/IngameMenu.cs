using UnityEngine;

public class IngameMenu : Singleton<IngameMenu>
{
    GameObject activeSubMenu;
    public GameObject menuCanvas;
    public GameObject backgroundCanvas;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuCanvas.activeSelf)
                ActivateTime();
            else
                Time.timeScale = 0;

            menuCanvas.SetActive(!menuCanvas.activeSelf);
            backgroundCanvas.SetActive(!backgroundCanvas.activeSelf);
        }    
    }

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

    public void ActivateTime()
    {
        Time.timeScale = 1;
    }
}
