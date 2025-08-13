using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AiMovement : MonoBehaviour
{
    public enum Faction
    {
        Friendly,
        Enemy
    }

    [Header("Identification")]
    public Faction aiFaction = Faction.Enemy;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public bool pauseAtPoints = true;
    public float pauseDuration = 1f;
    public float actionPauseDuration = 2f;
    public int panicMultiplier = 1;

    [Header("Pathfinding")]
    public List<Nodes> path = new List<Nodes>();

    private int currentIndex = 0;
    private bool movingForward = true; // for friendly ping-pong
    private Rigidbody2D rb;
    private bool isPaused = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning($"{name} has no path assigned!");
            enabled = false;
            return;
        }

        if (aiFaction == Faction.Enemy)
        {
            currentIndex = Random.Range(0, path.Count); // start at random node
        }
        else
        {
            currentIndex = 0;
            movingForward = true;
        }
    }

    private void FixedUpdate()
    {
        if (isPaused || path.Count == 0) return;

        if (aiFaction == Faction.Friendly)
            MoveAlongPath();
        else
            MoveRandomly();
    }

    // Ping-pong movement for friendly AI
    private void MoveAlongPath()
    {
        Nodes currentNode = path[currentIndex];
        Vector2 targetPos = currentNode.transform.position;
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPos, moveSpeed * panicMultiplier * Time.fixedDeltaTime));

        if ((rb.position - targetPos).sqrMagnitude < 0.04f)
        {
            StartCoroutine(HandlePauseAtNode(currentNode, true));
        }
    }

    // Random node movement for enemy AI
    private void MoveRandomly()
    {
        Nodes currentNode = path[currentIndex];
        Vector2 targetPos = currentNode.transform.position;
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPos, moveSpeed * panicMultiplier * Time.fixedDeltaTime));

        if ((rb.position - targetPos).sqrMagnitude < 0.04f)
        {
            StartCoroutine(HandlePauseAtNode(currentNode, false));
        }
    }

    // Handles pausing at nodes for both movement types
    private IEnumerator HandlePauseAtNode(Nodes node, bool isFriendly)
    {
        isPaused = true;

        // Determine wait time based on node tag
        float waitTime = pauseAtPoints ? pauseDuration : 0f;
        if (node.CompareTag("action"))
            waitTime = actionPauseDuration;

        yield return new WaitForSeconds(waitTime);

        // Friendly AI ping-pong movement
        if (isFriendly)
        {
            if (movingForward)
                currentIndex++;
            else
                currentIndex--;

            if (currentIndex >= path.Count)
            {
                currentIndex = path.Count - 2;
                movingForward = false;
            }
            else if (currentIndex < 0)
            {
                currentIndex = 1;
                movingForward = true;
            }
        }
        // Enemy AI random movement
        else
        {
            int newIndex;
            do
            {
                newIndex = Random.Range(0, path.Count);
            } while (newIndex == currentIndex); // avoid repeating same node
            currentIndex = newIndex;
        }

        isPaused = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (path == null || path.Count == 0) return;
        Gizmos.color = (aiFaction == Faction.Friendly) ? Color.blue : Color.red;

        for (int i = 0; i < path.Count; i++)
        {
            if (path[i] != null)
            {
                Gizmos.DrawSphere(path[i].transform.position, 0.2f);

                if (i < path.Count - 1 && path[i + 1] != null)
                    Gizmos.DrawLine(path[i].transform.position, path[i + 1].transform.position);
            }
        }
    }
}
