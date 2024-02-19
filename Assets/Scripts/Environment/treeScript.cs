using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TreeScript : Interactable
{
    public int progressFinish = 100;
    public int currentProgress;
    public TileBase tile;
    public GameObject droppedItem;

    public HealthBarScript healthBar;
    public GameObject healthCanvas;
    public Texture2D objectCursor;
    public Gathering playerGather;
    public NavMeshSurface2d navMesh;

    void Start()
    {
        currentProgress = 0;
        healthBar.SetMaxHealth(progressFinish);
    }

    protected virtual void OnMouseEnter()
    {
        Cursor.SetCursor(objectCursor, Vector2.zero, CursorMode.Auto);
    }

    protected virtual void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public override void Interact(UnityEngine.Transform interactingPlayer)
    {
        base.Interact(interactingPlayer);
        healthCanvas.SetActive(true);

        playerGather = interactingPlayer.GetComponent<Gathering>();
        
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

    public override void OnDeFocused(UnityEngine.Transform playerTransform)
    {
        base.OnDeFocused(playerTransform);
        if (playerList.Count <= 0)
        {
            healthCanvas.SetActive(false);
        }
    }
}
