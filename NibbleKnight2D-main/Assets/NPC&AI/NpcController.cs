using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NpcController : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int curHealth;
    public int panicMultiplier = 1;

    [Header("Pathfinding")]
    public Node currentNode;
    public List<Node> path = new List<Node>();
    // Keep a memory of last 3 patrol nodes
    private Queue<Node> lastPatrolNodes = new Queue<Node>();
    private int memorySize = 3;

    public enum FactionType
    {
        Friendly,
        Enemy
    }
    public FactionType faction = FactionType.Enemy; // default Enemy

    public enum StateMachine
    {
        Patrol,
        Engage,
        Evade
    }
    public StateMachine currentState;

    [Header("Movement")]
    public GameObject player;
    public float speed = 3f;
    public float stopDistance = 1.5f; // distance to stop from player when engaging

    [Header("Detection Box")]
    public Vector2 boxSize = new Vector2(5f, 5f); // width and height of detection box

    [Header("Patrol")]
    public float pauseAtNode = 0.5f;
    private float pauseTimer = 0f;

    private void Start()
    {
        curHealth = maxHealth;

        // Assign starting node
        if (currentNode == null)
        {
            currentNode = AStarManager.instance.FindNearestNode(transform.position);
        }
    }

    private void Update()
    {
        HandleStateTransitions();

        switch (currentState)
        {
            case StateMachine.Patrol:
                Patrol();
                break;
            case StateMachine.Engage:
                Engage();
                break;
            case StateMachine.Evade:
                Evade();
                break;
        }

        MoveAlongPath();
    }

    void HandleStateTransitions()
    {
        bool playerInBox = IsPlayerInDetectionBox();

        // Friendly NPCs only patrol
        if (faction == FactionType.Friendly)
        {
            if (currentState != StateMachine.Patrol)
            {
                currentState = StateMachine.Patrol;
                path.Clear();
            }
            return;
        }

        // Enemy NPCs
        if (curHealth <= maxHealth * 0.2f && currentState != StateMachine.Evade)
        {
            panicMultiplier = 2;
            currentState = StateMachine.Evade;
            path.Clear();
        }
        else if (playerInBox && curHealth > maxHealth * 0.2f && currentState != StateMachine.Engage)
        {
            panicMultiplier = 1;
            currentState = StateMachine.Engage;
            path.Clear();
        }
        else if (!playerInBox && curHealth > maxHealth * 0.2f && currentState != StateMachine.Patrol)
        {
            panicMultiplier = 1;
            currentState = StateMachine.Patrol;
            path.Clear();
        }
    }

    bool IsPlayerInDetectionBox()
    {
        Vector2 npcPos = transform.position;
        Vector2 playerPos = player.transform.position;

        return Mathf.Abs(playerPos.x - npcPos.x) <= boxSize.x * 0.5f &&
               Mathf.Abs(playerPos.y - npcPos.y) <= boxSize.y * 0.5f;
    }

    void Patrol()
    {
        if (path == null || path.Count == 0)
        {
            Node[] allNodes = AStarManager.instance.AllNodes();
            if (currentNode == null && allNodes.Length > 0)
                currentNode = allNodes[0];

            List<Node> possibleNodes = new List<Node>();

            // Build a list of nodes that are not in memory and not current node
            foreach (Node node in allNodes)
            {
                if (node != currentNode && !lastPatrolNodes.Contains(node))
                {
                    possibleNodes.Add(node);
                }
            }

            // If all nodes are in memory, allow them but exclude current node
            if (possibleNodes.Count == 0)
            {
                foreach (Node node in allNodes)
                {
                    if (node != currentNode)
                        possibleNodes.Add(node);
                }
            }

            // Pick a node randomly from possible nodes
            Node nextNode = possibleNodes[Random.Range(0, possibleNodes.Count)];

            // Update memory
            lastPatrolNodes.Enqueue(currentNode);
            if (lastPatrolNodes.Count > memorySize)
                lastPatrolNodes.Dequeue();

            // Generate path
            path = AStarManager.instance.GeneratePath(currentNode, nextNode) ?? new List<Node>();
        }
    }

    void Engage()
    {
        Vector2 playerPos = player.transform.position;
        float distanceToPlayer = Vector2.Distance(transform.position, playerPos);

        // Move toward player if farther than stopDistance
        if (distanceToPlayer > stopDistance)
        {
            Vector3 targetPos = new Vector3(playerPos.x, playerPos.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * panicMultiplier * Time.deltaTime);
        }
        else
        {
            // Close enough – stop
            // Optional: attack or do idle animation here
        }
    }

    void Evade()
    {
        if (path == null || path.Count == 0)
        {
            Node furthestNode = AStarManager.instance.FindFurthestNode(player.transform.position);
            path = AStarManager.instance.GeneratePath(currentNode, furthestNode) ?? new List<Node>();
        }
    }

    void MoveAlongPath()
    {
        if (path == null || path.Count == 0)
            return;

        Node nextNode = path[0];
        Vector3 targetPos = new Vector3(nextNode.transform.position.x, nextNode.transform.position.y, transform.position.z);

        // Smooth movement
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * panicMultiplier * Time.deltaTime);

        // Check if reached node (smooth stopping)
        if (Vector2.Distance(transform.position, nextNode.transform.position) < 0.05f)
        {
            currentNode = nextNode;
            path.RemoveAt(0);

            // Optional pause at nodes
            pauseTimer = pauseAtNode;
        }

        // Handle pause
        if (pauseTimer > 0)
        {
            pauseTimer -= Time.deltaTime;
            return; // skip movement while pausing
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection box
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // semi-transparent red
        Gizmos.DrawCube(transform.position, new Vector3(boxSize.x, boxSize.y, 0.1f));
    }
}
