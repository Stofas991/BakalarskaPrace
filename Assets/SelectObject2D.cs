using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SelectObject2D : MonoBehaviour
{
    // Update is called once per frame
    private bool isDragSelect = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private RaycastHit hit;
    [SerializeField] GameObject clickEffect;


    public RectTransform selectionBox;
    private HashSet<UnitSelection> selectedUnitList;
    [SerializeField] LayerMask clickableLayers;
    Interactable enemy;

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

        //vytvoøení selectovacího okna
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if(!isDragSelect && (startPosition - Input.mousePosition).magnitude > 30)
            {
                isDragSelect = true;
            }

            if(isDragSelect)
            {
                endPosition = Input.mousePosition;
                UpdateSelectionBox();
            }
        }

        //vybrání jednotek v selection boxu
        if(Input.GetMouseButtonUp(0))
        {
            if(isDragSelect)
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
        //po kliknutím pravým tlaèítkem, pokud máme vybranou jednotku posuneme na pozici myši
        if (Input.GetMouseButtonDown(1) && selectedUnitList.Count > 0)
        {
            //kontrola jestli se kliknulo na nepøítele
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.transform != null && hit.transform.CompareTag("Enemy"))
                enemy = hit.transform.GetComponent<Interactable>();
            else
                enemy = null;

            //particle effekt pro zobrazení bodu k pohybu
            if (clickEffect != null)
            {
                Vector3 positionZeroedZ = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                positionZeroedZ.z = 0;
                Instantiate(clickEffect, positionZeroedZ, clickEffect.transform.rotation);
            }

            foreach (UnitSelection unit in selectedUnitList)
            {
                //získání UnitControl property
                UnitControlScript UnitC = unit.gameObject.GetComponent<UnitControlScript>();
                //nastavení cílové lokace na pozici myši
                if (enemy == null)
                {
                    UnitC.enemy = null;
                    UnitC.target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                else
                {
                    UnitC.enemy = enemy;
                    UnitC.target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            
        }
    }

    /// <summary>
    /// aktivuje selection box a zobrazí obdélník podle pùvodní pozice myši a aktuální
    /// </summary>
    void UpdateSelectionBox()
    {
        selectionBox.gameObject.SetActive(isDragSelect);

        float width = endPosition.x - startPosition.x;
        float height = endPosition.y - startPosition.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        //ukotvuje na poèátku vytváøení obdélníku
        selectionBox.anchoredPosition = new Vector2(startPosition.x, startPosition.y) + new Vector2(width/2, height/2);
    }

    /// <summary>
    /// funkce najde všechny jednotky v selection boxu s tagem selectable
    /// </summary>
    /// <returns>seznam všech oznaèených jednotek</returns>
    HashSet<UnitSelection> SelectObjects(bool multipleUnits)
    {
        HashSet<UnitSelection> selectedUnits = new HashSet<UnitSelection>();

        foreach (UnitSelection unit in selectedUnitList)
        {
            unit.SetSelectedVisible(false);
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
                        unit.SetSelectedVisible(true);
                        selectedUnits.Add(unit);
                    }
                }
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.transform != null && hit.transform.CompareTag("selectable"))
            {
                UnitSelection unit = hit.transform.GetComponent<UnitSelection>();
                if (unit != null)
                {
                    unit.SetSelectedVisible(true);
                    selectedUnits.Add(unit);
                }
            }
        }
        return selectedUnits;
    }
}
