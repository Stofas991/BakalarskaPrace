using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowMouse : MonoBehaviour
{
    public RectTransform movingObject;
    public Vector3 offset;
    public RectTransform basisObject;
    public Camera cam;

    private void Update()
    {
        MoveObject();
    }

    public void MoveObject()
    {
        Vector3 pos = Input.mousePosition + offset;
        pos.z = basisObject.position.z;
        movingObject.position = cam.ScreenToWorldPoint(pos);
    }
}
