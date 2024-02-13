using System.Collections;
using System.Collections.Generic;
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
        myStats.cutCooldown -= Time.deltaTime;
    }

    public void CutTree(treeScript targetTree)
    {
        if (myStats.cutCooldown <= 0)
        {
            targetTree.TakeDamage(myStats.plantCutDamage);
            myStats.cutCooldown = 1f / myStats.plantCutSpeed;
            Debug.Log("hit for"+myStats.plantCutDamage);
        }
    }
}
