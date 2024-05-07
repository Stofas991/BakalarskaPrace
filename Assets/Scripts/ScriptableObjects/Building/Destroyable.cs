/**
* File: Destroyable.cs
* Author: Kryštof Glos
* Date Last Modified: 1.5.2024
* Description: This script defines behavior for destroyable objects, including taking damage, managing health, and destruction.
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Destroyable : MonoBehaviour, IAttackable
{
    public int maxHealth = 100;
    public HealthBarScript healthBar;
    public GameObject healthCanvas;
    private Tilemap objectTilemap;

    List<Transform> objectList = new List<Transform>();
    int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        objectTilemap = GameObject.Find("Walls").GetComponent<Tilemap>();
    }

    /// <summary>
    /// Function to handle taking damage.
    /// </summary>
    /// <param name="Damage">The amount of damage to take.</param>
    public void TakeDamage(int Damage)
    {
        currentHealth -= Damage;

        healthCanvas.SetActive(true);
        healthBar.SetHealth(currentHealth);
        // Play hurt animation

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    void Die()
    {
        //disable building
        objectTilemap.SetTile(new Vector3Int((int)transform.position.x, (int)transform.position.y, 0), null);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Destroy(gameObject);
    }

    /// <summary>
    /// Function called when the object is focused.
    /// </summary>
    /// <param name="focusedObject">The object that is focused.</param>
    public void OnFocused(Transform focusedObject)
    {
        if (!objectList.Contains(focusedObject))
            objectList.Add(focusedObject);

        healthCanvas.SetActive(true);
    }

    /// <summary>
    /// Function called when the object is defocused.
    /// </summary>
    /// <param name="focusedObject">The object that is defocused.</param>
    public virtual void OnDeFocused(Transform focusedObject)
    {
        objectList.Remove(focusedObject);
        if (objectList.Count == 0)
            healthCanvas.SetActive(false);
    }
}
