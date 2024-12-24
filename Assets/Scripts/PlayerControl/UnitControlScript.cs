/**
* File: UnitControlScript.cs
* Author: Kryštof Glos
* Date Last Modified: 3.5.2024
* Description: This script controls the behavior of a unit in the game, including movement, interaction, and resource handling.
*/

using System;
using UnityEngine;
using UnityEngine.AI;
using static Interactable;


public class UnitControlScript : MonoBehaviour
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
    private ActivityType activityType = ActivityType.None;

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
        if (focus == null)
        {
            switch (activityType)
            {
                case ActivityType.None:
                    break;
                case ActivityType.CuttingTrees:
                    FindNextTarget();
                    break;
                case ActivityType.Digging:
                    FindNextTarget();
                    break;
                case ActivityType.Hauling:
                    FindNextTarget();
                    break;
                case ActivityType.Farming:
                    FindNextTarget();
                    break;
                case ActivityType.Fighting:
                    FindNextTarget();
                    break;
            }
        }
    }

    // Handle user input
    private void HandleInput()
    {
        CancelAttack();

        if (Input.GetMouseButtonDown(1))
        {
            StopActivity();
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
                HaulItem();
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
    public bool PickUp(GameObject itemToPick, int count, ContainedItemType itemType)
    {
        if (carriedItem == null)
        {
            carriedItem = Instantiate(itemToPick, transform);
            stockpileZone = StockpileGetter();
            UCItemType = itemType;
            UCCount = count;
            HaulItem();
            return true;
        }
        else if (UCItemType == itemType)
        {
            UCCount += count;
            HaulItem();
            return true;
        }
        else
        {
            Debug.Log("Cant carry multiple types of resources");
            return false;
        }
    }

    // Haul item to the stockpile
    public void HaulItem()
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

    public void SetActivity(ActivityType activityType)
    {
        this.activityType = activityType;
    }

    public void StopActivity()
    {
        activityType = ActivityType.None;
    }

    private void FindNextTarget()
    {
        Interactable nextTarget = null;
        switch (activityType)
        {
            case ActivityType.None:
                break;
            case ActivityType.Farming:
                nextTarget = FindNearestObjectOfType(InteractableType.Farm);
                break;
            case ActivityType.Digging:
                nextTarget = FindNearestObjectOfType(InteractableType.Hill);
                break;
            case ActivityType.Hauling:
                nextTarget = FindNearestObjectOfType(InteractableType.Hauling);
                break;
            case ActivityType.CuttingTrees:
                nextTarget = FindNearestObjectOfType(InteractableType.Tree);
                break;
            case ActivityType.Fighting:
                nextTarget = FindNearestObjectOfType(InteractableType.Enemy);
                break;
        }
        if (nextTarget != null)
        {
            SetFocus(nextTarget);
        }
    }

    public Interactable FindNearestObjectOfType(InteractableType targetType)
    {
        float scanRadius = 10f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, scanRadius);
        GameObject nearestObject = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            Interactable interactable = collider.GetComponent<Interactable>();
            if (interactable != null && interactable.type == targetType)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestObject = collider.gameObject;
                }
            }
        }

        return nearestObject.GetComponent<Interactable>();
    }

    public enum ActivityType
    {
        Farming,
        CuttingTrees,
        Digging,
        Fighting,
        Hauling,
        None
    }
}