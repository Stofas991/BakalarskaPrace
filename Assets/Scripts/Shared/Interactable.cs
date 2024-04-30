using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public UnitStats myHealth { get; private set; }

    public float radius = 2f;

    bool isFocus = false;
    public bool hasInteracted = false;
    public bool isEnemy = false;
    public List<Transform> playerList;

    void Update()
    {
        if (isFocus && !hasInteracted && !isEnemy)
        {
            if (transform != null)
            {
                foreach (Transform player in playerList.ToList())
                {
                    float distance = Vector3.Distance(player.position, transform.position);
                    if (distance <= radius)
                    {
                        Interact(player);
                    }
                }
            }
        }
        if (isFocus && isEnemy)
        {
            if (transform != null)
            {
                foreach (Transform player in playerList.ToList())
                {
                    if (player == null)
                    {
                        playerList.Remove(player);
                        continue;
                    }
                    float distance = Vector3.Distance(player.position, transform.position);

                    if (distance <= player.GetComponent<UnitStats>().attackRange)
                    {
                        Interact(player);
                    }
                }
            }
        }
    }

    public virtual void Interact (Transform interactingPlayer)
    {
        //this method will be overritten by object that is going to use it
    }

    public void OnFocused (Transform playerTransform)
    {
        hasInteracted = false;
        isFocus = true;
        if (!playerList.Contains(playerTransform))
            playerList.Add(playerTransform);
    }

    public virtual void OnDeFocused(Transform playerTransform)
    {
        playerList.Remove(playerTransform);
        if (playerList.Count == 0)
        {
            hasInteracted = false;
            isFocus = false;
        }
    }

}
