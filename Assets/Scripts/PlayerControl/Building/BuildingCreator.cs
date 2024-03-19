using System.Collections;
using System.Collections.Generic;
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
    PlayerBuildInput playerInput;

    TileBase tileBase;
    BuildableObjectBase selectedObject;
    SelectObject2D SelectionScript;

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

        //set current tile to current mouse position tile
        previewMap.SetTile(currentGridPosition, tileBase);
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
                    DrawBounds(defaultMap);
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

        DrawBounds(previewMap);
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

        DrawBounds(previewMap);
    }

    private void DrawBounds(Tilemap map)
    {
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                map.SetTile(new Vector3Int(x, y, 0), tileBase);
            }
        }
    }

    private void DrawItem()
    {
        //todo: automatically select tilemap

        defaultMap.SetTile(currentGridPosition, tileBase);
    }
}
