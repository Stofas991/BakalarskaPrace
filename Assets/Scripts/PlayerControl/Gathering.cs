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

    public void GatherResource(ResourceGatherScript targetResource)
    {
        if (myStats.gatherCooldown <= 0)
        {
            targetResource.MakeProgress(myStats.gatheringDamage);
            myStats.gatherCooldown = 1f / myStats.gatheringSpeed;
        }
    }
}
