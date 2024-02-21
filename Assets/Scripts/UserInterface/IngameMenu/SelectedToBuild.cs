using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectedToBuild : MonoBehaviour
{
    public GameObject UIObject;
    public GameObject objectToPlace;
    SelectObject2D selectObject;
    GameObject currentPlaceableObject;

    void Start()
    {
        selectObject = UIObject.GetComponent<SelectObject2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlaceableObject != null)
        {
            MoveObject();
            ReleaseIfClicked();
        }
    }

    public virtual void Build()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        currentPlaceableObject = Instantiate(objectToPlace, mousePos, new Quaternion());
        selectObject.buildingMode = true;
    }

    private void MoveObject()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        currentPlaceableObject.transform.localPosition = mousePos;
    }

    private void ReleaseIfClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //check if it is possible to place object here
            //

            //some action to place blueprint
            //

            Debug.Log("Build it and lose it");

            selectObject.buildingMode = false;
            currentPlaceableObject = null;
        }
    }
}
