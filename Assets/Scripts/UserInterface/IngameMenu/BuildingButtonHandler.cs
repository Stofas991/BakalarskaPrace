using System.Collections;
using System.Collections.Generic;
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
