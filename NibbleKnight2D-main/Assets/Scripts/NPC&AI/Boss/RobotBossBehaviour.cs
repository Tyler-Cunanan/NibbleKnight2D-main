using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RobotBossBehaviour : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Moving,
        Dashing,
        Cooldown
    }

    public BossState currentState = BossState.Idle;

    [Header("Stats")]
    public int health = 0;
    public bool hasShield = false;

    [Header("Zone Settings")]
    public Vector2 boxSize = new Vector2(5f, 5f);
    public Color boxColor = new Color(0f, 1f, 0f, 0.25f);
    public LayerMask detectionLayer; //Step1: Add more layer in if you want it to detect more different objects. Step1

    [Header("Dash Settings")]
    public float dashDistance = 3f;
    public float dashSpeed = 10f;
    public float dashCooldown = 2f;

    private Vector3 dashStart;
    private Vector3 dashTarget;
    private float cooldownTimer = 0f;

    public GameObject playerMouse;

    public float moveSpeed = 2f;
    private Rigidbody2D rb;


    [Header("Other Components")]
    public Animator animator;

    public SwissHealthScript swissHealthScript;
    //public AudioSource dashSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                CheckZone();
                break;

            case BossState.Dashing:
                DashBackward();
                break;

            case BossState.Cooldown:
                HandleCooldown();
                break;
        }
    }

    void CheckZone()
    {
        Vector2 center = transform.position;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxSize, 0f, detectionLayer);
        
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Grabable"))
            {
                Debug.Log("Obstacle detected in zone!");
                StartDash();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player entered zone!");
            AttackPlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            animator.SetBool("Attack", false);

        }
    }

    void AttackPlayer()
    {
        animator.SetBool("Attack", true);
        Debug.Log("Boss is attacking.");
        swissHealthScript.SwissDamaged(10, transform);
    }

    void CreateShield()
    {
        animator.SetTrigger("CreateShield");
        Debug.Log("Boss is creating shield.");
        hasShield = true;
    }

    void StartDash()
    {
        currentState = BossState.Dashing;
        dashStart = transform.position;
        Vector3 dashDirection = -transform.right;
        dashTarget = dashStart + dashDirection * dashDistance;

        //if (animator != null)
        //{
        //    animator.SetTrigger("Dash");
        //}

        //if (dashSound != null)
        //{
        //    dashSound.Play();
        //}
    }

    void DashBackward()
    {
        transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, dashTarget) < 0.01f)
        {
            currentState = BossState.Cooldown;
            cooldownTimer = dashCooldown;
        }
    }

    void HandleCooldown()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            currentState = BossState.Idle;
        }
    }

    public void MoveToTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        Vector2 moveDir = direction.normalized;
        rb.velocity = moveDir * moveSpeed;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = boxColor;
        Gizmos.DrawCube(transform.position, boxSize);

        Color wireColor = boxColor;
        wireColor.a = 1f;
        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 dashDirection = -transform.right;
        Vector3 previewTarget = transform.position + dashDirection * dashDistance;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, previewTarget);
    }
}