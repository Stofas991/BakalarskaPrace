using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public UnitStats myHealth { get; private set; }

    public float radius = 3f;

    bool isFocus = false;
    bool hasInteracted = false;
    public bool isEnemy = false;
    public Transform player;

    void Update()
    {
        if (isFocus && !hasInteracted)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= radius)
            {
                Interact();
                hasInteracted = true;
            }
        }
        if (isFocus && isEnemy)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= player.GetComponent<UnitControlScript>().attackRange)
            {
                Interact();
            }
        }
    }

    public virtual void Interact ()
    {
        //tato metoda má být pøepsána objektem ktrý ji bude používat
    }

    public void OnFocused (Transform playerTransform)
    {
        hasInteracted = false;
        isFocus = true;
        player = playerTransform;
    }

    public void OnDeFocused()
    {
        hasInteracted = false;
        isFocus = false;
        player = null;
    }
}
