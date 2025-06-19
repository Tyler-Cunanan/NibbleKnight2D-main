using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public bool pauseAtPoints = true;
    public float pauseDuration = 1f;
    public float actionPauseDuration = 2f;

    [Header("Waypoints")]
    public List<Transform> waypoints;

    private Rigidbody2D rb;
    private Transform currentTarget;
    private bool isPaused = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogWarning("No waypoints assigned!");
            enabled = false;
            return;
        }

        PickNextTarget();
    }

    void FixedUpdate()
    {
        if (isPaused || currentTarget == null) return;

        Vector2 targetPos = currentTarget.position;
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector2.Distance(rb.position, targetPos) < 0.1f)
        {
            StartCoroutine(HandlePointReached(currentTarget));
        }
    }

    void PickNextTarget()
    {
        if (waypoints.Count == 0) return;

        Transform next;
        do
        {
            next = waypoints[Random.Range(0, waypoints.Count)];
        } while (next == currentTarget && waypoints.Count > 1); // Avoid immediate repeat unless only 1 point

        currentTarget = next;
    }

    IEnumerator HandlePointReached(Transform point)
    {
        isPaused = true;

        float waitTime = pauseAtPoints ? pauseDuration : 0f;

        if (point.CompareTag("action"))
        {
            waitTime = actionPauseDuration;
        }

        yield return new WaitForSeconds(waitTime);

        PickNextTarget();
        isPaused = false;
    }

    void OnDrawGizmosSelected()
    {
        // Visualize waypoints
        if (waypoints == null || waypoints.Count == 0) return;

        Gizmos.color = Color.yellow;
        foreach (Transform point in waypoints)
        {
            if (point != null)
                Gizmos.DrawSphere(point.position, 0.2f);
        }
    }
}
