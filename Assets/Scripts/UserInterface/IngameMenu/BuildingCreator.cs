using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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

    //if any one of those maps contain tile, cant place there
    [SerializeField] List<Tilemap> forbidPlacingMaps;

    TileBase tileBase;
    BuildableObjectBase selectedObject;
    SelectObject2D SelectionScript;
    GameObject categoryParent;


    Camera _camera;

    Vector2 mousePos;
    Vector3Int currentGridPosition;
    Vector3Int lastGridPosition;

    bool holdActive;
    bool isOverObject = false;
    Vector3Int holdStartPosition;

    BoundsInt bounds;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerBuildInput();
        SelectionScript = GetComponent<SelectObject2D>();
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
            bounds.xMin = currentGridPosition.x < holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
            bounds.xMax = currentGridPosition.x > holdStartPosition.x ? currentGridPosition.x : holdStartPosition.x;
            bounds.yMin = holdStartPosition.y;
            bounds.yMax = holdStartPosition.y;
        }
        else
        {
            bounds.xMin = holdStartPosition.x;
            bounds.xMax = holdStartPosition.x;
            bounds.yMin = currentGridPosition.y < holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;
            bounds.yMax = currentGridPosition.y > holdStartPosition.y ? currentGridPosition.y : holdStartPosition.y;
        }

        DrawBounds(previewMap, true);
    }

    private void DrawBounds(Tilemap map, bool previewMode)
    {
        //creating parent for each zone, if it is already created just use the one already existing
        if (!previewMode)
        {
            categoryParent = InitializeParent();
        }

        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                if (previewMode)
                {
                    map.SetTile(new Vector3Int(x, y, 0), tileBase);

                    if (IsForbidden(new Vector3Int(x, y, 0)))
                    {
                        if (!IsSameTilemap(new Vector3Int(x, y, 0), selectedObject.Category.tilemap))
                            map.SetColor(new Vector3Int(x, y, 0), Color.red);
                    }
                }
                else
                {
                    //cant place on forbidden tilemaps or on another zone
                    if (!IsForbidden(new Vector3Int(x, y, 0)) && !selectedObject.Category.tilemap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        map.SetTile(new Vector3Int(x, y, 0), tileBase);
                        GameObject item = Instantiate(selectedObject.Prefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                        item.transform.parent = categoryParent.transform;
                    }
                }
            }
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
            parent.transform.parent = itemParent.transform;
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
        if (!IsForbidden(currentGridPosition))
        {
            //placing tile and prefab
            tilemap.SetTile(currentGridPosition, tileBase);
            GameObject item = Instantiate(selectedObject.Prefab, new Vector3(currentGridPosition.x + 0.5f, currentGridPosition.y + 0.5f, 0), Quaternion.identity);
            item.transform.parent = itemParent.transform;

            //removing selected item and position
            selectedObject = null;
            holdActive = false;
            SelectionScript.startPosition = Input.mousePosition;
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
}

[System.Serializable]
public struct CategoryTilemap
{
    public BuildingCategory category;
    public Tilemap tilemap;
}
