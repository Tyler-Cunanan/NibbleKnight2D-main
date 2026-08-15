using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controls the basic behaviour of an NPC.
//
//  OVERVIEW:
// - Friendly NPCs: patrol between randomly selected navigation nodes.
// - Enemy NPCs: switch between Patrol, Engage, and Evade based on player proximity and current health.
// - AStarManager is responsible for finding/generating paths.
// - This script is responsible for deciding WHAT the NPC wants to do, while AStarManager is responsible for figuring out HOW to get there.
//
// AI INTEGRATION NOTE:
// This is currently a rule based state machine, not a machine learning system. An AI system could potentially replace or influence HandleStateTransitions(), GenerateRandomPath(),
//  or the choice of target node without necessarily rewriting the movement code.

public class NpcController : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int curHealth;

    // This is currently being used as a simple "panic" speed multiplier, rather than affecting the NPC's actual decision making.
    public int panicMultiplier = 1;

    [Header("Pathfinding")]
    // The navigation node the NPC currently considers itself to be on.
    // AStarManager uses these Nodes to calculate paths through the level.
    public Node currentNode;

    // The sequence of Nodes the NPC plans to visit.
    // path[0] = next destination. path[1] = destination after that, etc.
    public List<Node> path = new List<Node>();

    // Remembers recently visited patrol nodes.
    // This prevents the random patrol behaviour from repeatedly
    // selecting the same few nodes.
    private Queue<Node> lastPatrolNodes = new Queue<Node>();

    // Maximum number of recent patrol nodes to remember.
    private int memorySize = 3;

    public enum FactionType { Friendly, Enemy }
    public FactionType faction = FactionType.Enemy;

    public enum StateMachine { Patrol, Engage, Evade }
    public StateMachine currentState;

    [Header("Movement")]
    public GameObject player;
    public float speed = 3f;

    // When engaging the player, stop moving once this distance is reached.
    public float stopDistance = 1.5f;

    // Minimum and maximum random pause between patrol nodes.
    public float minPauseTime = 0.5f;
    public float maxPauseTime = 0.5f;
    //And start the Countdown
    public float pauseTimer = 0f;

    // ============================================================
    // PLAYER DETECTION
    // ============================================================
    [Header("Detection Box")]
    public Vector2 boxSize = new Vector2(5f, 5f);

    [Header("Other reference")]
    private EnemyNPCScript enemyNPCScript;
    private SwissHealthScript swissHealthScript;

    private void Start()
    {
        enemyNPCScript = gameObject.GetComponent<EnemyNPCScript>();
        swissHealthScript = gameObject.GetComponent<SwissHealthScript>();

        curHealth = maxHealth;

        if (currentNode == null)
            currentNode = AStarManager.instance.FindNearestNode(transform.position);
    }

    private void Update()
    {
        //Should have find a more optimal way for this instead FINDING the player every frame. Relatively expensive and unnecessary to perform every frame.
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

        // Friendly NPCs a random path right away and just move along with it path.
        if (faction == FactionType.Friendly)
        {
            path = GenerateRandomPath();
            Patrol();
        }
    }

    // ============================================================
    // STATE DECISION-MAKING
    // ============================================================
    void HandleStateTransitions()
    {
        // Check whether the player is currently inside the NPC's  detection box.
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

    // ============================================================
    // RANDOM PATROL PATH GENERATION
    // ============================================================
    //This utilized AStarManager.cs and Node.cs
    List<Node> GenerateRandomPath()
    {
        // Get every assigned navigation node available in the scene.
        Node[] allNodes = AStarManager.instance.AllNodes();
        if (currentNode == null && allNodes.Length > 0)
            currentNode = allNodes[0];

        List<Node> possibleNodes = new List<Node>();
        // Find nodes that:
        // 1. aren't our current node
        // 2. weren't recently visited
        foreach (Node node in allNodes)
        {
            if (node != currentNode && !lastPatrolNodes.Contains(node))
                possibleNodes.Add(node);
        }

        // If our memory filter removed everything, allow previously visited nodes again.
        if (possibleNodes.Count == 0)
        {
            foreach (Node node in allNodes)
            {
                if (node != currentNode)
                    possibleNodes.Add(node);
            }
        }

        // IMPORTANT:
        // Pick one of the valid destinations randomly.
        // This assumes possibleNodes.Count > 0. If the scene has only one Node, this can fail.
        Node nextNode = possibleNodes[Random.Range(0, possibleNodes.Count)];

        // Remember the current node so we don't immediately choose it again in future patrol decisions.
        lastPatrolNodes.Enqueue(currentNode);
        if (lastPatrolNodes.Count > memorySize)
            lastPatrolNodes.Dequeue();

        // AStarManager handles the actual pathfinding. If no path exists, return an empty list instead of null.
        return AStarManager.instance.GeneratePath(currentNode, nextNode) ?? new List<Node>();
    }

    // ============================================================
    // DEBUG VISUALIZATION
    // ============================================================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(boxSize.x, boxSize.y, 0.1f));
    }
}
