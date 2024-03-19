using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectedToBuild : MonoBehaviour
{
    [SerializeField] Tilemap previewMap;
    public GameObject UIObject;
    public GameObject tilemapParent;

    TileBase tileBase;
    Vector3Int currentGridPosition;
    Vector3Int lastGridPosition;
    public BuildableObjectBase selectedObject;

    private BuildableObjectBase SelectedObject
    {
        set
        {
            selectedObject = value;

            tileBase = selectedObject != null ? selectedObject.TileBase : null;

            UpdatePreview();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if something is selected, show preview
        if (selectedObject != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = previewMap.WorldToCell(mousePos);

            if(gridPos != currentGridPosition)
            {
                lastGridPosition = currentGridPosition;
                currentGridPosition = gridPos;
            }

            UpdatePreview();
        }
        
    }

    private void UpdatePreview()
    {
        //removing old tile if existing
        previewMap.SetTile(lastGridPosition, null);

        //set current tile to current mouse position tile
        previewMap.SetTile(currentGridPosition, tileBase);
    }
}
