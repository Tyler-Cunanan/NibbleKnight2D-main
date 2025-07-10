using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public Sprite customCursorSprite;     // Drag your custom cursor sprite (e.g., a dot) here
    public Vector2 cursorSize = new Vector2(0.1f, 0.1f); // Width and height in world units

    private GameObject cursorObject;

    void Start()
    {
        // Hide the system cursor
        Cursor.visible = false;

        // Create the custom cursor GameObject
        cursorObject = new GameObject("CustomCursor");
        SpriteRenderer sr = cursorObject.AddComponent<SpriteRenderer>();
        sr.sprite = customCursorSprite;

        // Set cursor size by scaling the GameObject
        cursorObject.transform.localScale = new Vector3(cursorSize.x, cursorSize.y, 1f);
    }

    void Update()
    {
        // Update custom cursor position to follow mouse
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorObject.transform.position = mousePosition;
    }
}
