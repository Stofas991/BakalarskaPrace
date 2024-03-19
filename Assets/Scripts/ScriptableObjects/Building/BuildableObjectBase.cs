using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Category
{
    Wall,
    Zone,
    Items
}

public enum PlaceType
{
    Single,
    Line,
    Rectangle
}

[CreateAssetMenu (fileName = "Buildable", menuName = "BuildingObjectBase/Create Buildable")]
public class BuildableObjectBase : ScriptableObject
{
    [SerializeField] Category category;
    [SerializeField] TileBase tileBase;
    [SerializeField] PlaceType placeType;

    public TileBase TileBase
    {
        get { return tileBase; }
    }

    public Category Category 
    { 
        get { return category; } 
    }

    public PlaceType PlaceType
    { 
        get { return placeType; } 
    }
}
