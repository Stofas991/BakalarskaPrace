using UnityEngine;
using UnityEngine.Tilemaps;

public class HexClickHandler : MonoBehaviour
{
    public Tilemap tilemap;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);

            Debug.Log("Klikl jsi na hexagon: " + cellPosition);
        }
    }
}
