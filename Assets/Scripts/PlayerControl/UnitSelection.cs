/**
* File: UnitSelection.cs
* Author: Kryštof Glos
* Date Last Modified: 18.2.2024
* Description: This script handles the visual indication of unit selection and updates the selected status in the associated UnitControlScript.
*/
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    private GameObject selectedGameObject;

    private void Awake()
    {
        selectedGameObject = transform.Find("Circle").gameObject;
        SetSelectedVar(false);
    }

    public void SetSelectedVar(bool selection)
    {
        selectedGameObject.SetActive(selection);
        UnitControlScript unit = gameObject.GetComponent<UnitControlScript>();
        unit.selected = selection;
    }
}
