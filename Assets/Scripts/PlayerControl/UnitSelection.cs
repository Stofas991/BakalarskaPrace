using System.Collections;
using System.Collections.Generic;
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
