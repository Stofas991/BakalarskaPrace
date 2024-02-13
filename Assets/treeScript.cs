using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class treeScript : Interactable
{
    public int progressFinish = 100;
    public int currentProgress;
    public TileBase tile;
    public GameObject droppedItem;

    public HealthBarScript healthBar;
    public GameObject healthCanvas;

    void Start()
    {
        currentProgress = 0;
        healthBar.SetMaxHealth(progressFinish);
    }

    public override void Interact()
    {
        base.Interact();
        healthCanvas.SetActive(true);

        Gathering playerGather = player.GetComponent<Gathering>();

        if (playerGather != null)
        {
            playerGather.CutTree(this);
        }
    }

    public void TakeDamage(int progress)
    {
        currentProgress += progress;
        healthBar.SetHealth(currentProgress);
        // Play hurt animation

        if (currentProgress >= progressFinish)
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
        GameObject wood = Instantiate(droppedItem, new Vector3(x + 0.5f, y + 0.5f, 0f), new Quaternion());

        wood.GetComponent<ItemSpecifics>().count = Random.Range(5, 20);

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
