/*
 * File: MainMenu.cs
 * Description: Handles functionality for the main menu UI, including setting game parameters and quitting the game.
 * Author: Kryštof Glos
 * Date: 6.5.2024
 */
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI seedText;
    [SerializeField] GameObject sizeToggleParent;
    [SerializeField] GameObject difficultyToggleParent;
    [SerializeField] GameObject generationTypeParent;

    [SerializeField] List<GameObject> easyPlayerUnits = new List<GameObject>();
    [SerializeField] List<GameObject> normalPlayerUnits = new List<GameObject>();
    [SerializeField] List<GameObject> hardPlayerUnits = new List<GameObject>();
    static SelectedValues SelectedValues = new SelectedValues();

    private void Start()
    {
        MusicScript.GetInstance().PlayMenuMusic();    
    }

    public void QuitGaime()
    {
        Application.Quit();
    }

    ///<summary>
    /// Generates a random seed value and displays it.
    ///</summary>
    public void randomSeed()
    {
        var randomValue = UnityEngine.Random.Range(100000000, 999999999);
        seedText.text = randomValue.ToString();
    }

    ///<summary>
    /// Sets the selected game parameters based on UI inputs.
    ///</summary>
    public void setValues()
    {
        SelectedValues.seed = Convert.ToInt32(seedText.text);
        var activeToggle = getActiveToggle(sizeToggleParent);
        switch (activeToggle.tag)
        {
            case "SmallMap":
                SelectedValues.mapSize = 50;
                break;
            case "MediumMap":
                SelectedValues.mapSize = 100;
                break;
            case "LargeMap":
                SelectedValues.mapSize = 200;
                break;
        }

        activeToggle = getActiveToggle(difficultyToggleParent);
        switch (activeToggle.tag)
        {
            case "SmallMap":
                SelectedValues.difficulty = 0.5f;
                SelectedValues.playerPrefabList = easyPlayerUnits;
                break;
            case "MediumMap":
                SelectedValues.difficulty = 1f;
                SelectedValues.playerPrefabList = normalPlayerUnits;
                break;
            case "LargeMap":
                SelectedValues.difficulty = 2f;
                SelectedValues.playerPrefabList = hardPlayerUnits;
                break;
        }

        activeToggle = getActiveToggle(generationTypeParent);
        switch(activeToggle.tag)
        {
            case "WFC":
                SelectedValues.isPerlin = false;
                break;
            case "Perlin":
                SelectedValues.isPerlin = true;
                break;

        }
        SelectedValues.isSet = true;
    }

    ///<summary>
    /// Retrieves the active toggle from a given toggle parent.
    ///</summary>
    ///<param name="toggleParent">The parent GameObject containing the toggles.</param>
    ///<returns>The active toggle.</returns>
    public Toggle getActiveToggle(GameObject toggleParent)
    {
        Toggle[] toggles = toggleParent.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles)
            if (t.isOn) return t;  //returns selected toggle
        return null;           // if nothing is selected return null
    }

    public void ChangeVolume(float volume)
    {
        MusicScript.GetInstance().ChangeVolume(volume);
    }
}

public class SelectedValues
{
    static public bool isSet = false;
    static public int seed;
    static public int mapSize;
    static public float difficulty;
    static public List<GameObject> playerPrefabList = new List<GameObject>();
    static public bool isPerlin = true;
}
