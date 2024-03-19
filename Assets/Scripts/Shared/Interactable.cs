using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public UnitStats myHealth { get; private set; }

    public float radius = 2f;

    bool isFocus = false;
    bool hasInteracted = false;
    public bool isEnemy = false;
    public List<Transform> playerList;

    void Update()
    {
        if (isFocus && !hasInteracted && !isEnemy)
        {
            if (transform != null)
            {
                foreach (Transform player in playerList)
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
                foreach (Transform player in playerList)
                {
                    if (player == null)
                    {
                        playerList.Remove(player);
                        continue;
                    }
                    float distance = Vector3.Distance(player.position, transform.position);

                    if (distance <= player.GetComponent<UnitControlScript>().attackRange)
                    {
                        Interact(player);
                    }
                }
            }
        }
    }

    public virtual void Interact (Transform interactingPlayer)
    {
        //tato metoda má být pøepsána objektem ktrý ji bude používat
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
        hasInteracted = false;
        isFocus = false;
        playerList.Remove(playerTransform);
    }

}
