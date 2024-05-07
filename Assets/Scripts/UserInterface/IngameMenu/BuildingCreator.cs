/**
* File: BuildingCreator.cs
* Author: Kryštof Glos
* Date Last Modified: 1.5.2024
* Description: This script handles the creation of buildings and zones in the game world.
*/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;

public class BuildingCreator : Singleton<BuildingCreator>
{
    [SerializeField] Tilemap previewMap, defaultMap;
    public CategoryTilemap[] categoriesTilemaps;
    PlayerBuildInput playerInput;
    [SerializeField] GameObject itemParent;
    [SerializeField] GameObject zoneParent;
    [SerializeField] NavMeshSurface2d nmSurface;

    //if any one of those maps contain tile, cant place there
    [SerializeField] List<Tilemap> forbidPlacingMaps;

    TileBase tileBase;
    BuildableObjectBase selectedObject;
    SelectObject2D SelectionScript;
    GameObject categoryParent;
    ResourcesScript resourceMenu;

    Camera _camera;

    Vector2 mousePos;
    Vector3Int currentGridPosition;
    Vector3Int lastGridPosition;

    bool holdActive;
    bool isOverObject = false;
    bool horizontalInverted = false;
    bool verticalInverted = false;
    Vector3Int holdStartPosition;

    BoundsInt bounds;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerBuildInput();
        SelectionScript = GetComponent<SelectObject2D>();
        resourceMenu = ResourcesScript.GetInstance();
        _camera = Camera.main;

        foreach (var item in categoriesTilemaps)
        {
            item.category.tilemap = item.tilemap;
        }
    }

    private BuildableObjectBase SelectedObject
    {
        set
        {
            selectedObject = value;

            tileBase = selectedObject != null ? selectedObject.TileBase : null;

            SelectionScript.buildingMode = true;

            UpdatePreview();
        }
    }

    private Tilemap tilemap
    {
        get
        {
            if (selectedObject != null && selectedObject.Category != null && selectedObject.Category.tilemap != null)
            {
                return selectedObject.Category.tilemap;
            }

            return defaultMap;
        }
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.Building.MousePosition.performed += OnMouseMove;

        playerInput.Building.MouseLeftClick.performed += OnLeftClick;
        playerInput.Building.MouseLeftClick.started += OnLeftClick;
        playerInput.Building.MouseLeftClick.canceled += OnLeftClick;

        playerInput.Building.MouseRightClick.performed += OnRightClick;
    }

    private void OnDisable()
    {
        playerInput.Disable();

        playerInput.Building.MousePosition.performed -= OnMouseMove;

        playerInput.Building.MouseLeftClick.performed -= OnLeftClick;
        playerInput.Building.MouseLeftClick.started -= OnLeftClick;
        playerInput.Building.MouseLeftClick.canceled -= OnLeftClick;

        playerInput.Building.MouseRightClick.performed -= OnRightClick;
    }

    private void Update()
    {
        // Check if a building object is selected
        if (selectedObject != null)
        {
            // Check if the mouse is over a UI element
            isOverObject = EventSystem.current.IsPointerOverGameObject();
            Vector3 pos = _camera.ScreenToWorldPoint(mousePos);
            Vector3Int gridPos = previewMap.WorldToCell(pos);

            if (gridPos != currentGridPosition)
            {
                lastGridPosition = currentGridPosition;
                currentGridPosition = gridPos;

                UpdatePreview();

                if (holdActive)
                {
                    HandleDrawing();
                }
            }
        }
        else
        {
            SelectionScript.buildingMode = false;
        }
    }

    ///<summary>
    /// Update the position of the mouse pointer.
    ///</summary>
    private void OnMouseMove(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
    }

    ///<summary>
    /// Handle left mouse button clicks for building placement.
    ///</summary>
    private void OnLeftClick(InputAction.CallbackContext ctx)
    {
        if (selectedObject != null && !isOverObject)
        {
            if (ctx.phase == InputActionPhase.Started)
            {
                holdActive = true;

                if (ctx.interaction is TapInteraction)
                {
                    holdStartPosition = currentGridPosition;
                }
                HandleDrawing();
            }
            else
            {
                //performed or canceled
                if (ctx.interaction is SlowTapInteraction || ctx.interaction is TapInteraction && ctx.phase == InputActionPhase.Performed)
                {
                    holdActive = false;
                    //draw on release
                    HandleDrawRelease();
                }
                else if (ctx.interaction is TapInteraction && ctx.phase == InputActionPhase.Performed)
                {
                    //This makes it so rectangles and lines can be placed on click
                    HandleDrawRelease();
                }
            }
        }
    }

    ///<summary>
    /// Handle right mouse button clicks to cancel building placement.
    ///</summary>
    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        // Deselect the currently selected object, clear the preview, and reset drawing state
        SelectedObject = null;
        previewMap.ClearAllTiles();
        holdActive = false;
        SelectionScript.startPosition = Input.mousePosition;
    }

    public void ObjectSelected(BuildableObjectBase obj)
    {
        SelectedObject = obj;
    }

    ///<summary>
    /// Updates the preview of the building based on the current mouse position.
    ///</summary>
    private void UpdatePreview()
    {
        //removing old tile if existing
        previewMap.SetTile(lastGridPosition, null);

        // Set the tile at the current mouse position
        previewMap.SetTile(currentGridPosition, tileBase);

        // Highlight the tile in red if it is forbidden
        if (IsForbidden(currentGridPosition))
        {
            previewMap.SetTileFlags(currentGridPosition, TileFlags.None);
            previewMap.SetColor(currentGridPosition, Color.red);
        }
    }

    ///<summary>
    /// Handles drawing the building preview based on the selected building type.
    ///</summary>
    private void HandleDrawing()
    {
        if (selectedObject != null)
        {
            horizontalInverted = false;
            verticalInverted = false;
            switch (selectedObject.PlaceType)
            {
                case PlaceType.Single:
                default:
                    DrawItem();
                    break;
                case PlaceType.Line:
                    LineRenderer();
                    break;
                case PlaceType.Rectangle:
                    RectangleRenderer();
                    break;
            }
        }
    }

    ///<summary>
    /// Handles the release of the drawing action for the selected building type.
    ///</summary>
    private void HandleDrawRelease()
    {
        if (selectedObject != null)
        {
            switch (selectedObject.PlaceType)
            {
                case PlaceType.Single:
                    DrawItem();
                    break;
                case PlaceType.Line:
                case PlaceType.Rectangle:
                    DrawBounds(tilemap, false);
                    previewMap.ClearAllTiles();
                    break;
            }
        }
    }

    ///<summary>
    /// Renders the rectangle shape of the building preview.
    ///</summary>
    private void RectangleRenderer()
    {
        previewMap.ClearAllTiles();

        bounds.xMin = currentGridPosition.x < holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
        bounds.xMax = currentGridPosition.x > holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
        bounds.yMin = currentGridPosition.y < holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;
        bounds.yMax = currentGridPosition.y > holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;

        DrawBounds(previewMap, true);
    }

    ///<summary>
    /// Renders the line shape of the building preview.
    ///</summary>
    private void LineRenderer()
    {
        previewMap.ClearAllTiles();

        // Determine if the line is horizontal or vertical
        float diffX = Mathf.Abs(currentGridPosition.x - holdStartPosition.x);
        float diffY = Mathf.Abs(currentGridPosition.y - holdStartPosition.y);

        bool lineIsHorizontal = diffX >= diffY;

        if (lineIsHorizontal)
        {
            // Calculate bounds for horizontal line
            if (currentGridPosition.x < holdStartPosition.x)
            {
                bounds.xMin = currentGridPosition.x;
                horizontalInverted = true;
                verticalInverted = false;
            }
            else
            {
                bounds.xMin = holdStartPosition.x;
                horizontalInverted = false;
                verticalInverted = false;
            }
            bounds.xMax = currentGridPosition.x > holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
            bounds.yMin = holdStartPosition.y;
            bounds.yMax = holdStartPosition.y;
        }
        else
        {
            // Calculate bounds for vertical line
            bounds.xMin = holdStartPosition.x;
            bounds.xMax = holdStartPosition.x;
            if (currentGridPosition.y < holdStartPosition.y)
            {
                bounds.yMin = currentGridPosition.y;
                verticalInverted = true;
                horizontalInverted = false;
            }
            else
            {
                bounds.yMin = holdStartPosition.y;
                verticalInverted = false;
                horizontalInverted = false;
            }
            bounds.yMax = currentGridPosition.y > holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;
        }

        DrawBounds(previewMap, true);
    }

    ///<summary>
    /// Draws the building bounds on the tilemap.
    ///</summary>
    ///<param name="map">The target tilemap.</param>
    ///<param name="previewMode">Indicates whether the drawing is in preview mode.</param>
    private void DrawBounds(Tilemap map, bool previewMode)
    {
        // Initialize dictionaries to track required resources
        Dictionary<ContainedItemType, int> requiredResourcesPreview = new Dictionary<ContainedItemType, int>();
        Dictionary<ContainedItemType, int> requiredResourcesUsed = new Dictionary<ContainedItemType, int>();

        // Create parent object for each zone, if it doesn't already exist
        if (!previewMode)
        {
            categoryParent = InitializeParent();
        }

        int xIndex = 0, yIndex = 0;
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                var requiredResources = selectedObject.RequiredResources;
                int posX, posY;

                //calculation of total resource cost
                if (requiredResources.itemType != ContainedItemType.None)
                {
                    if (requiredResourcesPreview.ContainsKey(requiredResources.itemType))
                        requiredResourcesPreview[requiredResources.itemType] += requiredResources.ammount;
                    else
                        requiredResourcesPreview[requiredResources.itemType] = requiredResources.ammount;

                    if (resourceMenu.resourceList.Any(x => x.requiredResources.itemType == requiredResources.itemType && x.requiredResources.ammount >= requiredResources.ammount))
                    {
                        if (requiredResourcesUsed.ContainsKey(requiredResources.itemType))
                            requiredResourcesUsed[requiredResources.itemType] += requiredResources.ammount;
                        else
                            requiredResourcesUsed[requiredResources.itemType] = requiredResources.ammount;
                    }
                }
                if (horizontalInverted)
                    posX = bounds.xMax - xIndex;
                else
                    posX = x;

                if (verticalInverted)
                    posY = bounds.yMax - yIndex;
                else
                    posY = y;

                if (previewMode)
                {
                    map.SetTile(new Vector3Int(posX, posY, 0), tileBase);

                    // Highlight forbidden tiles or tiles with insufficient resources in red
                    if (IsForbidden(new Vector3Int(posX, posY, 0)) || NotEnoughResources(requiredResourcesPreview))
                    {
                        if (!IsSameTilemap(new Vector3Int(posX, posY, 0), selectedObject.Category.tilemap))
                            map.SetColor(new Vector3Int(posX, posY, 0), Color.red);
                    }
                }
                else
                {

                    // Place the building and update resource status
                    Vector3Int targetPosition = new Vector3Int(posX, posY, 0);
                    previewMap.SetTile(targetPosition, null);
                    if (!IsForbidden(targetPosition) && !selectedObject.Category.tilemap.HasTile(targetPosition) && !NotEnoughResources(requiredResourcesPreview))
                    {
                        //updating status of resource menu
                        resourceMenu.UpdateAmmount(-requiredResources.ammount, requiredResources.itemType);
                        if (requiredResourcesPreview.ContainsKey(requiredResources.itemType))
                            requiredResourcesPreview[requiredResources.itemType] -= requiredResources.ammount;

                        map.SetTile(targetPosition, tileBase);
                        GameObject item = Instantiate(selectedObject.Prefab, new Vector3(posX + 0.5f, posY + 0.5f, 0), Quaternion.identity);
                        item.transform.parent = categoryParent.transform;
                    }
                }
                yIndex++;
            }
            xIndex++;
        }
    }

    ///<summary>
    /// Initializes the parent object for the selected building category.
    ///</summary>
    ///<returns>The initialized parent object.</returns>
    private GameObject InitializeParent()
    {
        GameObject parent;
        if (selectedObject.Category.name == "Zone")
        {
            parent = GameObject.Find(selectedObject.name + "_zone");
            if (parent == null)
            {
                parent = new GameObject(selectedObject.name + "_zone");
            }
            parent.transform.parent = zoneParent.transform;
        }
        else if (selectedObject.Category.name == "Wall")
        {
            parent = GameObject.Find(selectedObject.name + "_wall");
            if (parent == null)
            {
                parent = new GameObject(selectedObject.name + "_wall");
            }
            parent.transform.parent = itemParent.transform;
        }
        else
        {
            parent = null;
        }
        return parent;
    }

    ///<summary>
    /// Draws a single building item.
    ///</summary>
    private void DrawItem()
    {
        List<RequiredResources> resourcesToUpdate = new List<RequiredResources>();
        if (!IsForbidden(currentGridPosition))
        {
            var requiredResources = selectedObject.RequiredResources;

            //checking if all needed resources are present
            foreach (var resource in resourceMenu.resourceList)
            {
                if (resource.requiredResources.itemType == requiredResources.itemType)
                {
                    if (resource.requiredResources.ammount >= requiredResources.ammount)
                    {
                        resourcesToUpdate.Append(resource.requiredResources);
                    }
                    else
                    {
                        Debug.Log("not enough " + resource.requiredResources.itemType);
                        RemoveSelectedItem();
                        return;
                    }
                }
            }
            foreach (var resource in resourcesToUpdate)
            {
                resourceMenu.UpdateAmmount(resource.ammount, resource.itemType);
            }

            // Place the tile and prefab
            tilemap.SetTile(currentGridPosition, tileBase);
            GameObject item = Instantiate(selectedObject.Prefab, new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0), Quaternion.identity);
            item.transform.parent = itemParent.transform;

            RemoveSelectedItem();
        }

    }

    ///<summary>
    /// Checks if the current position is on a forbidden tile.
    ///</summary>
    ///<param name="pos">The position to check.</param>
    ///<returns>True if the position is on a forbidden tile, otherwise false.</returns>
    private bool IsForbidden(Vector3Int pos)
    {
        return forbidPlacingMaps.Any(map =>
        {
            return map.HasTile(pos);
        });
    }

    ///<summary>
    /// Checks if the current position is on the same tilemap as the specified tilemap.
    ///</summary>
    ///<param name="pos">The position to check.</param>
    ///<param name="tilemapToCheck">The tilemap to compare against.</param>
    ///<returns>True if the position is on the same tilemap, otherwise false.</returns>
    private bool IsSameTilemap(Vector3Int pos, Tilemap tilemapToCheck)
    {
        foreach (Tilemap map in forbidPlacingMaps)
        {
            if (map.HasTile(pos))
            {
                if (map == tilemapToCheck)
                    return true;
            }
        }
        return false;
    }

    ///<summary>
    /// Removes the selected item and resets the drawing state.
    ///</summary>
    private void RemoveSelectedItem()
    {
        previewMap.SetTile(currentGridPosition, null);
        selectedObject = null;
        holdActive = false;
        SelectionScript.startPosition = Input.mousePosition;
    }

    ///<summary>
    /// Checks if there are insufficient resources based on the specified resource dictionary.
    ///</summary>
    ///<param name="resources">The dictionary containing required resources.</param>
    ///<returns>True if there are insufficient resources, otherwise false.</returns>
    private bool NotEnoughResources(Dictionary<ContainedItemType, int> resources)
    {
        foreach (var resource in resourceMenu.resourceList)
        {
            if (resources.ContainsKey(resource.requiredResources.itemType))
            {
                if (resources[resource.requiredResources.itemType] > resource.requiredResources.ammount)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

[System.Serializable]
public struct CategoryTilemap
{
    public BuildingCategory category;
    public Tilemap tilemap;
}
