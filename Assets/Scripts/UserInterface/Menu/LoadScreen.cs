/*
 * File: LoadScreen.cs
 * Description: Handles the loading screen functionality for transitioning between scenes.
 * Author: Kryštof Glos
 * Date: 6.5.2024
 */
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    public GameObject loadingScreen;
    public Image loadingBarFill;

    ///<summary>
    /// Loads the specified scene asynchronously.
    ///</summary>
    ///<param name="sceneId">The index of the scene to load.</param>
    public void LoadScene(int sceneId)
    {
        Time.timeScale = 1;
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    ///<summary>
    /// Loads the scene asynchronously and displays a loading screen with progress bar.
    ///</summary>
    ///<param name="sceneId">The index of the scene to load.</param>
    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBarFill.fillAmount = progressValue;

            yield return null;
        }
    }
}
