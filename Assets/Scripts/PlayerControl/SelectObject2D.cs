using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectObject2D : MonoBehaviour
{
    // Update is called once per frame
    private bool isDragSelect = false;
    public Vector3 startPosition;
    private Vector3 endPosition;

    public RectTransform selectionBox;
    public bool buildingMode = false;
    private HashSet<UnitSelection> selectedUnitList;

    private void Awake()
    {
        selectedUnitList = new HashSet<UnitSelection>();
    }

    void Update()
    {
        if (selectedUnitList.Count > 0)
        {
            HashSet<UnitSelection> tmp = new HashSet<UnitSelection>();
            foreach (var unit in selectedUnitList)
            {
                if (unit != null)
                {
                    tmp.Add(unit);
                }
            }
            selectedUnitList = tmp;
        }

        if (!buildingMode)
        {
            //creating selection box
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                if (!isDragSelect && (startPosition - Input.mousePosition).magnitude > 30)
                {
                    isDragSelect = true;
                }

                if (isDragSelect)
                {
                    endPosition = Input.mousePosition;
                    UpdateSelectionBox();
                }
            }

            //selecting units inside box
            if (Input.GetMouseButtonUp(0))
            {
                if (isDragSelect)
                {
                    selectedUnitList = SelectObjects(isDragSelect);
                    isDragSelect = false;
                    UpdateSelectionBox();
                }
                else
                {
                    selectedUnitList = SelectObjects(isDragSelect);
                }
            }
        }
    }

    /// <summary>
    /// aktivates selection box i range of mouse start and end position
    /// </summary>
    void UpdateSelectionBox()
    {
        selectionBox.gameObject.SetActive(isDragSelect);

        float width = endPosition.x - startPosition.x;
        float height = endPosition.y - startPosition.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        //anchoring box to start
        selectionBox.anchoredPosition = new Vector2(startPosition.x, startPosition.y) + new Vector2(width / 2, height / 2);
    }

    /// <summary>
    /// finds all units in selection box and selects them
    /// </summary>
    /// <returns>list of selected units</returns>
    HashSet<UnitSelection> SelectObjects(bool multipleUnits)
    {
        HashSet<UnitSelection> selectedUnits = new HashSet<UnitSelection>();

        HashSet<UnitSelection> unitCopy = new HashSet<UnitSelection>();

        foreach (UnitSelection unit in selectedUnitList)
        {
            unitCopy.Add(unit);
            unit.SetSelectedVar(false);
        }

        selectedUnitList.Clear();

        if (multipleUnits)
        {
            Vector2 minValue = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
            Vector2 maxValue = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

            GameObject[] selectableObjs = GameObject.FindGameObjectsWithTag("selectable");

            foreach (GameObject selectedObj in selectableObjs)
            {
                Vector3 objScreenPos = Camera.main.WorldToScreenPoint(selectedObj.transform.position);

                if (objScreenPos.x > minValue.x && objScreenPos.x < maxValue.x && objScreenPos.y > minValue.y && objScreenPos.y < maxValue.y)
                {
                    UnitSelection unit = selectedObj.GetComponent<UnitSelection>();
                    if (unit != null)
                    {
                        unit.SetSelectedVar(true);
                        selectedUnits.Add(unit);
                    }
                }
            }
        }
        else
        {
            if (EventSystem.current.IsPointerOverGameObject() && unitCopy.Count != 0)
            {
                foreach (var unit in unitCopy)
                {
                    unit.SetSelectedVar(true);
                }
                return unitCopy;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.transform != null && hit.transform.CompareTag("selectable"))
            {
                UnitSelection unit = hit.transform.GetComponent<UnitSelection>();
                if (unit != null)
                {
                    unit.SetSelectedVar(true);
                    selectedUnits.Add(unit);
                }
            }
        }
        return selectedUnits;
    }
}
