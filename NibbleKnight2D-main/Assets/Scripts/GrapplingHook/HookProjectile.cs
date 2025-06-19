using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private GrappleHook parentScript;
    private LayerMask hookableLayer;
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;
    private Transform playerTransform;

    [Header("Arc Settings")]
    public float gravityScale = 1.5f;
    public float maxLifetime = 2f;

    private float lifetime;

    public void Initialize(Vector2 dir, float spd, GrappleHook parent, LayerMask hookMask)
    {
        direction = dir.normalized;
        speed = spd;
        parentScript = parent;
        hookableLayer = hookMask;
        playerTransform = parent.transform;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.velocity = direction * speed;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;

        lifetime = maxLifetime;
    }

    void FixedUpdate()
    {
        lifetime -= Time.fixedDeltaTime;
        if (lifetime <= 0f)
        {
            SnapBack();
        }

        DrawLine();
    }

    void DrawLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, playerTransform.position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hookableLayer) != 0)
        {
            parentScript.StartGrappleFromProjectile(other.transform.position, other.gameObject);
        }
        else
        {
            SnapBack();
        }

        Destroy(gameObject);
    }

    void SnapBack()
    {
        parentScript.FailGrapple();
        Destroy(gameObject);
    }
}
