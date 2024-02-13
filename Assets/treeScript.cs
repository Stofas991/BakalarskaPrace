using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class treeScript : Interactable
{
    public int maxHealth = 100;
    public int currentHealth;
    public TileBase tile;
    public GameObject droppedItem;
    void Start()
    {
        currentHealth = maxHealth;
    }

    public override void Interact()
    {
        base.Interact();

        Gathering playerGather = player.GetComponent<Gathering>();

        if (playerGather != null)
        {
            playerGather.CutTree(this);
        }
    }

    public void TakeDamage(int Damage)
    {
        currentHealth -= Damage;
        //HealthBar.SetHealth(CurrentHealth);
        // Play hurt animation

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    void Die()
    {
        //die animation

        //disable tree, drop wood
        Destroy(gameObject);
        GameObject trees = GameObject.FindGameObjectWithTag("TreeTilemap");
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        trees.GetComponent<Tilemap>().SetTile(new Vector3Int(x, y, 0), tile);
        Instantiate(droppedItem, new Vector3(x + 0.5f, y + 0.5f, 0f), new Quaternion()); 

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
