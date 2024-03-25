using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu (fileName = "Buildable", menuName = "Building/Create Buildable")]
public class BuildableObjectBase : ScriptableObject
{
    [SerializeField] BuildingCategory category;
    [SerializeField] TileBase tileBase;
    [SerializeField] PlaceType placeType;
    [SerializeField] UICategory uiCategory;
    [SerializeField] GameObject prefab;

    public TileBase TileBase
    {
        get { return tileBase; }
    }

    public BuildingCategory Category 
    { 
        get { return category; } 
    }

    public PlaceType PlaceType
    { 
        get { return placeType == PlaceType.None ? category.PlaceType : placeType; } 
    }
    //Write about this, every buildable object has placeType, can be None, if it is none it gets value from BuildingCategory which also has placeType

    public UICategory UICategory
    {
        get { return uiCategory; }
    }

    public GameObject Prefab
    {
        get { return prefab; }
    }
}
