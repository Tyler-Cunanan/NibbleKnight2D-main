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
    private Queue<Node> lastPatrolNodes = new Queue<Node>();
    private int memorySize = 3;

    public enum FactionType { Friendly, Enemy }
    public FactionType faction = FactionType.Enemy;

    public enum StateMachine { Patrol, Engage, Evade }
    public StateMachine currentState;

    [Header("Movement")]
    public GameObject player;
    public float speed = 3f;
    public float stopDistance = 1.5f;
    public float minPauseTime = 0.5f;
    public float maxPauseTime = 0.5f;
    public float pauseTimer = 0f;

    [Header("Detection Box")]
    public Vector2 boxSize = new Vector2(5f, 5f);


    private void Start()
    {
        curHealth = maxHealth;

        if (currentNode == null)
            currentNode = AStarManager.instance.FindNearestNode(transform.position);

        // Friendly NPCs get a random path right away
        if (faction == FactionType.Friendly)
        {
            path = GenerateRandomPath();
        }
    }

    private void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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

        // Friendly NPCs just move along their path
        if (faction == FactionType.Friendly)
        {
            MoveAlongPath();
        }
    }

    void HandleStateTransitions()
    {
        bool playerInBox = IsPlayerInDetectionBox();

        if (faction == FactionType.Friendly)
            return; // Friendly NPC never changes state

        // Enemy NPC logic
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
        float randomPauseDuration = Random.Range(minPauseTime, maxPauseTime);
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        if (path == null || path.Count == 0)
        {
            path = GenerateRandomPath();
        }

        MoveAlongPath();
    }

    void Engage()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > stopDistance)
        {
            Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * panicMultiplier * Time.deltaTime);
        }
        // else: close enough — can attack or idle
    }

    void Evade()
    {
        if (path == null || path.Count == 0)
        {
            Node furthestNode = AStarManager.instance.FindFurthestNode(player.transform.position);
            path = AStarManager.instance.GeneratePath(currentNode, furthestNode) ?? new List<Node>();
        }

        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        if (path == null || path.Count == 0)
            return;

        Node nextNode = path[0];
        Vector2 targetPos = new Vector2(nextNode.transform.position.x, nextNode.transform.position.y);

        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * panicMultiplier * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextNode.transform.position) < 0.05f)
        {
            currentNode = nextNode;
            path.RemoveAt(0);
            float randomPauseDuration = Random.Range(minPauseTime, maxPauseTime);
            pauseTimer = randomPauseDuration;
        }
    }

    List<Node> GenerateRandomPath()
    {
        Node[] allNodes = AStarManager.instance.AllNodes();
        if (currentNode == null && allNodes.Length > 0)
            currentNode = allNodes[0];

        List<Node> possibleNodes = new List<Node>();
        foreach (Node node in allNodes)
        {
            if (node != currentNode && !lastPatrolNodes.Contains(node))
                possibleNodes.Add(node);
        }

        if (possibleNodes.Count == 0)
        {
            foreach (Node node in allNodes)
            {
                if (node != currentNode)
                    possibleNodes.Add(node);
            }
        }

        Node nextNode = possibleNodes[Random.Range(0, possibleNodes.Count)];
        lastPatrolNodes.Enqueue(currentNode);
        if (lastPatrolNodes.Count > memorySize)
            lastPatrolNodes.Dequeue();

        return AStarManager.instance.GeneratePath(currentNode, nextNode) ?? new List<Node>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(boxSize.x, boxSize.y, 0.1f));
    }
}
