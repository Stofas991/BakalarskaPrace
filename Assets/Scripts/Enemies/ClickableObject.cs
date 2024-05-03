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
