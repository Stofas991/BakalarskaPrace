using Sentry;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public float panSpeed = 20f;
    public float minSpeed, maxSpeed;
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
    private Vector3 origin, difference;
    private bool drag = false;

    // Update is called once per frame

    void Update()
    {
        halfHeight = cam.orthographicSize;
        halfWidth = cam.aspect * halfHeight;

        Vector3 pos = transform.position;

        // Handle keyboard input for panning
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

        // Handle middle mouse button drag movement
        if (Input.GetMouseButton(2))
        {
            difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
            if (!drag)
            {
                drag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SentrySdk.CaptureMessage("test message");
            }
        }
        else
        {
            drag = false;
        }

        if (drag)
        {
            pos = origin - difference;
        }

        scroll -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        scroll = Mathf.Clamp(scroll, minZ, maxZ);
        panSpeed = scroll * 2;
        panSpeed = Mathf.Clamp(panSpeed, minSpeed, maxSpeed);

        // Clamp camera position to stay within the map bounds
        pos.x = Mathf.Clamp(pos.x, 0 + halfWidth, panLimit.x - halfWidth);
        pos.y = Mathf.Clamp(pos.y, 0 + halfHeight, panLimit.y - halfHeight);

        cam.orthographicSize = scroll;
        transform.position = pos;
    }

    // Method to set camera borders based on map size
    public void setCameraBorders()
    {
        panLimit.y = Generator.mapHeight;
        panLimit.x = Generator.mapWidth;
        maxZ = Generator.mapHeight / 4;
        minSpeed = 20f;
        maxSpeed = Generator.mapHeight / 2;
        transform.position = new Vector3(Generator.mapWidth / 2, Generator.mapHeight / 2, -1);
    }
}
