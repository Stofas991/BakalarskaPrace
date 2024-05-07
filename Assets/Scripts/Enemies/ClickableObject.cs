/**
 * File: ClickableObject.cs
 * Author: Kryštof Glos
 * Last Modified: 18.2.2024
 * Description: Script for changing cursor when hovering over clickable objects them.
 */

using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public Texture2D objectCursor;

    protected virtual void OnMouseEnter()
    {
        Cursor.SetCursor(objectCursor, Vector2.zero, CursorMode.Auto);
    }

    protected virtual void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
