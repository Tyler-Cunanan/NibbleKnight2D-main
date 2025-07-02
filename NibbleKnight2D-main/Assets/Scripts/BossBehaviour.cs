using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    [Header("Zone Settings")]
    public Vector2 boxSize = new Vector2(5f, 5f);
    public Color boxColor = new Color(0f, 1f, 0f, 0.25f);  // Default: translucent green
    // LayerMask to filter objects (optional)
    public LayerMask detectionLayer;

    [Header("Dash Settings")]
    public float dashDistance = 3f;
    public float dashSpeed = 10f;
    private bool isDashing = false;
    private Vector3 dashStart;
    private Vector3 dashTarget;
    public float dashCooldown = 2f; // Time in seconds before the next dash
    private float cooldownTimer = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        if (isDashing)
        {
            DashBackward();
        }
        else
        {
            // Update cooldown timer if needed
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= Time.deltaTime;
            }
            else
            {
                CheckZone();
            }
        }
    }

    void CheckZone()
    {
        Vector2 center = transform.position;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxSize, 0f, detectionLayer);

        if (hits.Length > 0)
        {
            StartDash();
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashStart = transform.position;

        // Dash backward relative to this object's local right direction
        Vector3 backward = -transform.right * dashDistance;
        dashTarget = dashStart + backward;
    }

    void DashBackward()
    {
        transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, dashTarget) < 0.01f)
        {
            isDashing = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = boxColor;
        Gizmos.DrawCube(transform.position, boxSize);

        Color wireColor = boxColor;
        wireColor.a = 1f; // Full opacity for wireframe
        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
