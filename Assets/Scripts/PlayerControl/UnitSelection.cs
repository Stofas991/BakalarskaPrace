/**
* File: UnitSelection.cs
* Author: Kryštof Glos
* Date Last Modified: 18.2.2024
* Description: This script handles the visual indication of unit selection and updates the selected status in the associated UnitControlScript.
*/
using UnityEngine;
using UnityEngine.UI;

public class UnitSelection : MonoBehaviour
{
    private GameObject selectedGameObject;
    private CharacterUI characterUI;
    private UnitStats stats;

    private void Awake()
    {
        selectedGameObject = transform.Find("Circle").gameObject;
        characterUI = CharacterUI.GetInstance();
        stats = GetComponent<UnitStats>();
        SetSelectedVar(false);
    }

    public void SetSelectedVar(bool selection)
    {
        selectedGameObject.SetActive(selection);
        UnitControlScript unit = gameObject.GetComponent<UnitControlScript>();
        Sprite characterImage = gameObject.GetComponent<SpriteRenderer>().sprite;
        unit.selected = selection;
        if (selection)
        {
            characterUI.ShowBoxForCharacter(stats, characterImage);
        }
    }
}
