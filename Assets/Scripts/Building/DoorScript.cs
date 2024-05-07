/**
 * File: CameraController.cs
 * Author: Kryštof Glos
 * Date: 7.5.2024
 * Description: This script handles the behavior of the doors in the game.
 */

using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorScript : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase doorTile;

    void Start()
    {
        tilemap = GameObject.FindGameObjectWithTag("WallTilemap").GetComponent<Tilemap>();
        CheckDoors(new Vector3Int((int)transform.position.x, (int)transform.position.y, 0));
    }

    void CheckDoors(Vector3Int position)
    {
        // Check if there is a wall above and below the tile
        bool hasWallAbove = tilemap.GetTile(position + Vector3Int.up) != null;
        bool hasWallBelow = tilemap.GetTile(position + Vector3Int.down) != null;

        // If there is a wall above and below or left and right, return true
        if (hasWallAbove || hasWallBelow)
        {
            tilemap.SetTile(position, doorTile);
        }
    }
}
