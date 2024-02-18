using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class controllerKamery : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public MapGenerator Generator;
    public float scrollSpeed = 2f;
    public float scroll;
    float halfHeight;
    float halfWidth;
    public Camera cam;
    public float maxZ = 25f;
    public float minZ = 5f;

    private void Start()
    {
        panLimit.y = Generator.mapHeight;
        panLimit.x = Generator.mapWidth;
    }

    // Update is called once per frame

    void Update()
    {
        halfHeight = cam.orthographicSize;
        halfWidth = cam.aspect * halfHeight;

        Vector3 pos = transform.position;

        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.y += panSpeed * Time.deltaTime;

        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.y -= panSpeed * Time.deltaTime;

        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;

        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;

        }

        scroll -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        scroll = Mathf.Clamp(scroll, minZ, maxZ);


        pos.x = Mathf.Clamp(pos.x, 0+halfWidth, panLimit.x-halfWidth);
        pos.y = Mathf.Clamp(pos.y, 0+halfHeight, panLimit.y-halfHeight);

        cam.orthographicSize = scroll;
        transform.position = pos;
    }


}
