using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Destroyable : MonoBehaviour
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

    public void TakeDamage(int Damage)
    {
        currentHealth -= Damage;

        healthBar.SetHealth(currentHealth);
        // Play hurt animation

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    void Die()
    {
        //die animation

        //disable enemy
        Destroy(gameObject);
        objectTilemap.SetTile(new Vector3Int ((int)transform.position.x, (int)transform.position.y, 0), null);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnFocused(Transform focusedObject)
    {
        if (!objectList.Contains(focusedObject))
            objectList.Add(focusedObject);

        healthCanvas.SetActive(true);
    }

    public virtual void OnDeFocused(Transform focusedObject)
    {
        objectList.Remove(focusedObject);
        if (objectList.Count == 0)
            healthCanvas.SetActive(false);
    }
}
