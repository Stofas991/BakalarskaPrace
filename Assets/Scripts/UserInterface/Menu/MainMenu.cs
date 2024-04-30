using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI seedText;
    [SerializeField] GameObject sizeToggleParent;
    [SerializeField] GameObject difficultyToggleParent;
    static SelectedValues SelectedValues = new SelectedValues();
    public void PlayGame()
    {
        setValues();
        SceneManager.LoadScene("DefaultScene");
    }

    public void QuitGaime()
    {
        Debug.Log("quit");
    }

    public void randomSeed()
    {
        var randomValue = UnityEngine.Random.Range(100000000, 999999999);
        seedText.text = randomValue.ToString();
    }

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
                break;
            case "MediumMap":
                SelectedValues.difficulty = 1f;
                break;
            case "LargeMap":
                SelectedValues.difficulty = 2f;
                break;
        }
        SelectedValues.isSet = true;
    }

    public Toggle getActiveToggle(GameObject toggleParent)
    {
        Toggle[] toggles = toggleParent.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles)
            if (t.isOn) return t;  //returns selected toggle
        return null;           // if nothing is selected return null
    }
}

public class SelectedValues
{
    static public bool isSet = false;
    static public int seed;
    static public int mapSize;
    static public float difficulty;
}
