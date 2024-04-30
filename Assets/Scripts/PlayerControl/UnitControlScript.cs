using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;


public class UnitControlScript: MonoBehaviour
{
    [SerializeField] private GameObject clickEffect;
    [SerializeField] private GameObject zoneParent;

    private Vector3 target;
    private NavMeshAgent agent;
    private Animator animator;
    private Interactable focus;
    private PlayerMotor motor;
    private GameObject stockpileZone;
    private GameObject carriedItem;
    private UnitStats stats;

    // Unit properties
    public int UCCount;
    public ContainedItemType UCItemType = ContainedItemType.None;
    public bool selected = false;

    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        target = agent.transform.position;
        motor = GetComponent<PlayerMotor>();
        animator = GetComponent<Animator>();
        stats = GetComponent<UnitStats>();
        zoneParent = GameObject.FindGameObjectWithTag("ZoneParent");
    }

    // Update is called once per frame
    private void Update()
    {
        if (selected)
        {
            HandleInput();
        }
    }

    // Handle user input
    private void HandleInput()
    {
        CancelAttack();

        if (Input.GetMouseButtonDown(1))
        {
            ProcessMouseInput();
        }
    }

    // Process mouse input
    private void ProcessMouseInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit)
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            StockpileScript stockpile = interactable?.GetComponent<StockpileScript>();

            if (stockpile != null && UCItemType != ContainedItemType.None)
            {
                HaulItem(UCCount, UCItemType);
            }
            else if (interactable != null && stockpile == null)
            {
                SetFocus(interactable);
            }
        }
        else
        {
            MoveToClickPoint();
        }
    }

    // Move to the point clicked by the user
    private void MoveToClickPoint()
    {
        RemoveFocus();
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        motor.MoveToPoint(target);

        // Instantiate click effect
        if (clickEffect != null)
        {
            Vector3 positionZeroedZ = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            positionZeroedZ.z = 0;
            Instantiate(clickEffect, positionZeroedZ, clickEffect.transform.rotation);
        }
    }

    // Reset attack animation
    private void CancelAttack()
    {
        animator?.ResetTrigger("InRange");
    }

    // Set focus on the given interactable object
    private void SetFocus(Interactable newFocus)
    {
        if (newFocus != focus)
        {
            if (focus != null)
            {
                focus.OnDeFocused(transform);
            }
            focus = newFocus;
            focus.OnFocused(transform);
            motor.FollowTarget(focus, stats.attackRange);
        }
    }

    // Remove focus from the current interactable object
    public void RemoveFocus()
    {
        if (focus != null)
            focus.OnDeFocused(transform);
        focus = null;
        motor.StopFollowingTarget();
    }

    // Pick up an item and move it to the stockpile
    public void PickUp(GameObject itemToPick, int count, ContainedItemType itemType)
    {
        carriedItem = Instantiate(itemToPick, transform);
        stockpileZone = StockpileGetter();
        UCItemType = itemType;
        UCCount = count;
        HaulItem(count, itemType);
    }

    // Haul item to the stockpile
    public void HaulItem(int count, ContainedItemType itemType)
    {
        StockpileScript stockpileScript = FindEmptyStockpile();

        if (stockpileScript != null)
        {
            SetFocus(stockpileScript);
        }
        else
        {
            Debug.Log("No space in the stockpile");
        }
    }

    // Find an empty stockpile to haul item
    private StockpileScript FindEmptyStockpile()
    {
        foreach (Transform child in stockpileZone.transform)
        {
            StockpileScript stockpileScript = child.GetComponent<StockpileScript>();

            if (!stockpileScript.containsItem || (stockpileScript.itemType == UCItemType && stockpileScript.itemCount + UCCount <= stockpileScript.itemMaxCount))
            {
                return stockpileScript;
            }
        }

        return null;
    }

    // Destroy the carried item
    public void DestroyCarriedItem()
    {
        Destroy(carriedItem);
    }

    // Get the reference to the stockpile zone
    public GameObject StockpileGetter()
    {
        Transform stockpile = zoneParent.transform.Find("Stockpile_zone");
        return stockpile?.gameObject;
    }
}