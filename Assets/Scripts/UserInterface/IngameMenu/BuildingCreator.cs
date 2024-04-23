using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
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
        if (selectedObject != null)
        {
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

    private void OnMouseMove(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
    }

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

    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        SelectedObject = null;
        previewMap.ClearAllTiles();
        holdActive = false;
        SelectionScript.startPosition = Input.mousePosition;
    }

    public void ObjectSelected(BuildableObjectBase obj)
    {
        SelectedObject = obj;
    }

    private void UpdatePreview()
    {
        //removing old tile if existing
        previewMap.SetTile(lastGridPosition, null);

        previewMap.SetTile(currentGridPosition, tileBase);

        if (IsForbidden(currentGridPosition))
        {
            previewMap.SetTileFlags(currentGridPosition, TileFlags.None);
            previewMap.SetColor(currentGridPosition, Color.red);
        }
    }

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

    private void HandleDrawRelease()
    {
        if (selectedObject != null)
        {
            switch (selectedObject.PlaceType)
            {
                case PlaceType.Line:
                case PlaceType.Rectangle:
                    DrawBounds(tilemap, false);
                    previewMap.ClearAllTiles();
                    break;
            }
        }
    }

    private void RectangleRenderer()
    {
        previewMap.ClearAllTiles();

        bounds.xMin = currentGridPosition.x < holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
        bounds.xMax = currentGridPosition.x > holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
        bounds.yMin = currentGridPosition.y < holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;
        bounds.yMax = currentGridPosition.y > holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;

        DrawBounds(previewMap, true);
    }

    private void LineRenderer()
    {
        previewMap.ClearAllTiles();

        float diffX = Mathf.Abs(currentGridPosition.x - holdStartPosition.x);
        float diffY = Mathf.Abs(currentGridPosition.y - holdStartPosition.y);

        bool lineIsHorizontal = diffX >= diffY;

        if (lineIsHorizontal)
        {
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

    private void DrawBounds(Tilemap map, bool previewMode)
    {
        Dictionary<ContainedItemType, int> requiredResourcesPreview = new Dictionary<ContainedItemType, int>();
        Dictionary<ContainedItemType, int> requiredResourcesUsed = new Dictionary<ContainedItemType, int>();
        //creating parent for each zone, if it is already created just use the one already existing
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

                    if (IsForbidden(new Vector3Int(posX, posY, 0)) || NotEnoughResources(requiredResourcesPreview))
                    {
                        if (!IsSameTilemap(new Vector3Int(posX, posY, 0), selectedObject.Category.tilemap))
                            map.SetColor(new Vector3Int(posX, posY, 0), Color.red);
                    }
                }
                else
                {

                    //cant place on forbidden tilemaps or on another zone
                    previewMap.SetTile(new Vector3Int(posX, posY, 0), null);
                    if (!IsForbidden(new Vector3Int(posX, posY, 0)) && !selectedObject.Category.tilemap.HasTile(new Vector3Int(posX, posY, 0)) && !NotEnoughResources(requiredResourcesPreview))
                    {
                        //updating status of resource menu
                        resourceMenu.UpdateAmmount(-requiredResources.ammount, requiredResources.itemType);
                        if (requiredResourcesPreview.ContainsKey(requiredResources.itemType))
                            requiredResourcesPreview[requiredResources.itemType] -= requiredResources.ammount;

                        map.SetTile(new Vector3Int(posX, posY, 0), tileBase);
                        GameObject item = Instantiate(selectedObject.Prefab, new Vector3(posX + 0.5f, posY + 0.5f, 0), Quaternion.identity);
                        item.transform.parent = categoryParent.transform;
                    }
                }
                yIndex++;
            }
            xIndex++;
        }
    }

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

            //placing tile and prefab
            tilemap.SetTile(currentGridPosition, tileBase);
            GameObject item = Instantiate(selectedObject.Prefab, new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0), Quaternion.identity);
            item.transform.parent = itemParent.transform;

            RemoveSelectedItem();
        }

    }

    private bool IsForbidden (Vector3Int pos)
    {
        return forbidPlacingMaps.Any(map =>
        {
            return map.HasTile(pos);
        });
    }

    //maybe create override method to IsForbidden
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

    //removing selected item and position
    private void RemoveSelectedItem()
    {
        previewMap.SetTile(currentGridPosition, null);
        selectedObject = null;
        holdActive = false;
        SelectionScript.startPosition = Input.mousePosition;
    }

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
