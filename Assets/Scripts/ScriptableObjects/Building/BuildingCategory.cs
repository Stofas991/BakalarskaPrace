using System.Collections;
using System.Collections.Generic;
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
