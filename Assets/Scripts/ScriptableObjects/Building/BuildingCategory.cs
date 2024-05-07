/**
* File: BuildingCategory.cs
* Author: Kryštof Glos
* Date Last Modified: 20.3.2024
* Description: This script defines the category of a buildable object, including the type of placing that determines how it can be positioned.
*/
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PlaceType
{
    None,
    Single,
    Line,
    Rectangle
}

[CreateAssetMenu(fileName = "Category", menuName = "Building/Create Category")]
public class BuildingCategory : ScriptableObject
{
    [SerializeField] PlaceType placeType;
    public Tilemap tilemap;

    public PlaceType PlaceType
    {
        get { return placeType; }
    }
}
