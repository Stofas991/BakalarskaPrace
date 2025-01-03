using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapCameraController : MonoBehaviour
{
    public float panSpeed = 10f;
    public HexMapGenerator hexMapGenerator;
    private Vector2 panLimit;
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 15f;

    public Camera cam;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("Kamera nebyla nalezena! Ujisti se, že má tag MainCamera.");
                return;
            }
        }
        panLimit = new Vector2(hexMapGenerator.tilemap.size.x / 2, hexMapGenerator.tilemap.size.y / 2);
    }

    void Update()
    {
        if (cam == null) return;

        Vector3 pos = transform.position;

        if (Input.GetMouseButton(2))
        {
            float moveX = -Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime * cam.orthographicSize;
            float moveY = -Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime * cam.orthographicSize;

            pos.x += moveX;
            pos.y += moveY;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, -panLimit.y, panLimit.y);

        transform.position = pos;
    }
}
