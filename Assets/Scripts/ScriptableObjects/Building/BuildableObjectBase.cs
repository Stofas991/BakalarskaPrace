/**
* File: BuildableObjectBase.cs
* Author: Kryštof Glos
* Date Last Modified: 1.5.2024
* Description: This script defines the properties of a buildable object that can be placed in the game world.
*/
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Buildable", menuName = "Building/Create Buildable")]
public class BuildableObjectBase : ScriptableObject
{
    [SerializeField] BuildingCategory category;
    [SerializeField] TileBase tileBase;
    [SerializeField] PlaceType placeType;
    [SerializeField] UICategory uiCategory;
    [SerializeField] GameObject prefab;
    [SerializeField] RequiredResources requiredResources;
    [SerializeField] bool destroyer = false;

    public TileBase TileBase
    {
        get { return tileBase; }
    }

    public BuildingCategory Category
    {
        get { return category; }
    }

    /// <summary>
    /// The type of placing how the buildable object can be placed.
    /// </summary>
    public PlaceType PlaceType
    {
        get { return placeType == PlaceType.None ? category.PlaceType : placeType; }
    }

    public UICategory UICategory
    {
        get { return uiCategory; }
    }

    public GameObject Prefab
    {
        get { return prefab; }
    }

    public RequiredResources RequiredResources
    {
        get { return requiredResources; }
    }

    public bool Destroyer
    {
        get { return destroyer; }
    }
}

[System.Serializable]
public class RequiredResources
{
    public ContainedItemType itemType;
    public int ammount;
}