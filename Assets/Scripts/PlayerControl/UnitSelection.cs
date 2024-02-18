using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    private GameObject selectedGameObject;

    private void Awake()
    {
        selectedGameObject = transform.Find("Circle").gameObject;
        SetSelectedVisible(false);
    }

    public void SetSelectedVisible(bool visible)
    {
        selectedGameObject.SetActive(visible);
    }

    public void SetSelectedVar(bool selection)
    {
        UnitControlScript unit = gameObject.GetComponent<UnitControlScript>();
        unit.selected = selection;
    }
}
