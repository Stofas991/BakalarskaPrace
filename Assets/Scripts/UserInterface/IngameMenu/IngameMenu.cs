/*
 * File: IngameMenu.cs
 * Author: Kryštof Glos
 * Date: 7.5.2024
 * Description: Manages the in-game menu functionality.

 */
using UnityEngine;
using UnityEngine.SceneManagement;

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

    ///<summary>
    /// Activates the specified submenu and closes any previously active submenu.
    ///</summary>
    ///<param name="subMenu">The submenu to activate.</param>
    public void ActivateSubMenu(GameObject subMenu)
    {
        CloseActiveMenu(subMenu);

        subMenu.SetActive(!subMenu.activeSelf);    //activating new submenu
        activeSubMenu = subMenu;
    }

    ///<summary>
    /// Closes the active submenu if it is not the same as the provided menu.
    ///</summary>
    ///<param name="menu">The menu to check against the active submenu.</param>
    void CloseActiveMenu(GameObject menu)
    {
        if (activeSubMenu != null && activeSubMenu != menu)
        {
            activeSubMenu.SetActive(false);
            activeSubMenu = null;
        }
    }

    ///<summary>
    /// Restores the time scale to normal (1).
    ///</summary
    public void ActivateTime()
    {
        Time.timeScale = 1;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ChangeVolume(float volume)
    {
        MusicScript.GetInstance().ChangeVolume(volume);
    }
}
