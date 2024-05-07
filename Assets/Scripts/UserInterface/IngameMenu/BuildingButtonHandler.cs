/**
* File: BuildingButtonHandler.cs
* Author: Kryštof Glos
* Date Last Modified: 20.3.2024
* Description: This script handles the behavior of building buttons in the UI.
*/
using UnityEngine;
using UnityEngine.UI;

public class BuildingButtonHandler : MonoBehaviour
{
    [SerializeField] BuildableObjectBase item;
    Button button;

    BuildingCreator buildingCreator;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonClicked);
        buildingCreator = BuildingCreator.GetInstance(); //singleton
    }

    public BuildableObjectBase Item
    {
        set
        {
            item = value;
        }
    }

    private void ButtonClicked()
    {
        buildingCreator.ObjectSelected(item);
    }
}
