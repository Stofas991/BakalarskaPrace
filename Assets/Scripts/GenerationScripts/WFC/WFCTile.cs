/**
* File: WFCTile.cs
* Author: Kry�tof Glos
* Date Last Modified: 23.4.2024
* Description: This script defines properties and behaviors of individual tiles used in the Wave Function Collapse algorithm.
*/
using System.Collections.Generic;
using UnityEngine;

public class WFCTile : MonoBehaviour
{
    public int weight;
    public TypeWeight typeAndWeight = new TypeWeight();
    public TileType tileType;
    public WFCTile[] upNeighbours;
    public WFCTile[] downNeighbours;
    public WFCTile[] rightNeighbours;
    public WFCTile[] leftNeighbours;

    public Dictionary<Direction, WFCTile[]> directionNeighbours = new Dictionary<Direction, WFCTile[]>();

    /// <summary>
    /// Sets the values of direction neighbours.
    /// </summary>
    public void SetValues()
    {
        if (directionNeighbours.Count == 0)
        {
            directionNeighbours.Add(Direction.Up, upNeighbours);
            directionNeighbours.Add(Direction.Down, downNeighbours);
            directionNeighbours.Add(Direction.Left, leftNeighbours);
            directionNeighbours.Add(Direction.Right, rightNeighbours);
        }
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public enum TileType
{
    Grass,
    Mountain,
    Water,
    Beach
}

