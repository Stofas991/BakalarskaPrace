/**
* File: Gathering.cs
* Author: Kryštof Glos
* Date Last Modified: 23.4.2024
* Description: This script handles the gathering behavior of a unit.
*/
using UnityEngine;

public class Gathering : MonoBehaviour
{
    UnitStats myStats;

    void Start()
    {
        myStats = GetComponent<UnitStats>();
    }

    void Update()
    {
        myStats.gatherCooldown -= Time.deltaTime;
    }

    /// <summary>
    /// Initiates the gathering of a resource.
    /// </summary>
    /// <param name="targetResource">The resource to gather.</param>
    public void GatherResource(ResourceGatherScript targetResource)
    {
        if (myStats.gatherCooldown <= 0)
        {
            targetResource.MakeProgress(myStats.gatheringDamage);
            myStats.gatherCooldown = 1f / myStats.gatheringSpeed;
        }
    }
}
