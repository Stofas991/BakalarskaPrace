/**
 * File: ResourceGatherScript.cs
 * Author: Kryštof Glos
 * Last Modified: 3.5.2024
 * Description: Script for controlling resource gathering interaction and progress.
 */
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceGatherScript : Interactable
{
    public int progressFinish = 100;
    public int currentProgress;
    public TileBase tile;
    public GameObject droppedItem;

    public HealthBarScript healthBar;
    public GameObject healthCanvas;
    public Texture2D objectCursor;
    public Gathering playerGather;
    public ContainedItemType resourceType;

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

        if (playerGather != null && transform != null)
        {
            playerGather.GatherResource(this);
        }
    }

    public void MakeProgress(int progress)
    {
        currentProgress += progress;
        healthBar.SetHealth(currentProgress);
        // Play hurt animation

        if (currentProgress >= progressFinish)
        {
            switch (resourceType)
            {
                case ContainedItemType.Wood:
                    Die("TreeTilemap");
                    break;
                case ContainedItemType.Stone:
                    Die("WallTilemap");
                    break;
            }
        }

    }

    void Die(string tilemap)
    {
        //disable tree, drop wood
        Destroy(gameObject);
        GameObject tilemapObject = GameObject.FindGameObjectWithTag(tilemap);
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        tilemapObject.GetComponent<Tilemap>().SetTile(new Vector3Int(x, y, 0), tile);
        GameObject item = Instantiate(droppedItem, new Vector3(x + 0.5f, y + 0.5f, 0f), new Quaternion());

        item.GetComponent<ItemSpecifics>().count = Random.Range(5, 20);

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
