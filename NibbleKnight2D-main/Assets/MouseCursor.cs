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
        // Hide system cursor
        Cursor.visible = false;

        // Create cursor object
        cursorObject = new GameObject("CustomCursor");

        SpriteRenderer sr = cursorObject.AddComponent<SpriteRenderer>();
        sr.sprite = customCursorSprite;

        // Make cursor always render in front
        sr.sortingLayerName = "UI";   // Optional: use a top layer
        sr.sortingOrder = 9999;       // Very high order

        // Set size
        cursorObject.transform.localScale = new Vector3(cursorSize.x, cursorSize.y, 1f);
    }

    void Update()
    {
        // Update custom cursor position to follow mouse
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorObject.transform.position = mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("click");
            Debug.Log(Input.mousePosition);
        }
    }
}
