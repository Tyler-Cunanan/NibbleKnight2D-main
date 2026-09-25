using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using TMPro;

// Controls the Robot Boss's movement, combat, shield, and basic behaviour.
//
// OVERVIEW:
// The boss currently has four states:
//     Idle       -> Look for obstacles / decide whether to dash
//     Moving     -> Defined, but currently not used by Update()
//     Dashing    -> Move backward toward dashTarget
//     Cooldown   -> Defined, but currently not processed by Update()
//
// The boss also:
// - Detects objects inside a rectangular zone.
// - Dashes backward when it detects a "Grabable" object.
// - Attacks the player when they enter/stay inside its trigger.
// - Takes damage from "Grabable" objects.
// - Creates a shield when health reaches 50 or below.
// - Updates temporary boss health/shield UI.
//
// Potential future AI decision points include:
// - Which state to enter.
// - When/how to attack.
// - Whether to dash.
// - Where to move.
// - When to create/use the shield.
public class RobotBossBehaviour : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Moving,
        Dashing,
        Cooldown
    }

    // IMPORTANT:
    // Current behaviour state of the boss.    
    // Idle and Dashing currently have behaviour in Update(). Moving and Cooldown exist in the enum but currently have no corresponding behaviour in Update().
    public BossState currentState = BossState.Idle;

    [Header("Stats")]
    public int health = 0;
    public int shieldAmount = 0;
    public bool hasShield;

    [Header("Zone Settings")]
    public Vector2 boxSize = new Vector2(5f, 5f);
    public Color boxColor = new Color(0f, 1f, 0f, 0.25f);

    // Determines which physics layers can be detected by
    // Physics2D.OverlapBoxAll().
    //
    // IMPORTANT FOR AI TEAM MEMBER:
    // This is a LayerMask, not a Tag. The detection query first filters using this mask, and CheckZone() then specifically checks for "Grabable".
    public LayerMask detectionLayer; //Step1: Add more layer in if you want it to detect more different objects.

    [Header("Dash Settings")]
    public float dashDistance = 3f;
    public float dashSpeed = 10f;
    public float dashCooldown = 2f;

    private Vector3 dashStart;
    private Vector3 dashTarget;
    private float cooldownTimer = 0f;

    //Grab the player character... Sorry if the naming is ass.
    public GameObject playerMouse;

    public float moveSpeed = 2f;
    private Rigidbody2D rb;

    [Header("Damage Stun / Invulnerability")]
    public float damageInvulnerabilityDuration = 10f;
    private bool damageInvulnerable = false;
    private int playerLayer;
    private int bossLayer;

    [Header("Other Components")]
    public Animator animator;

    public SwissHealthScript swissHealthScript;

    public GameObject bossZone;

    //Temporary UI for Boss Health
    public TextMeshProUGUI healthDisplayText;
    public TextMeshProUGUI shieldDisplayText;
    //public AudioSource dashSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        playerLayer = LayerMask.NameToLayer("Player");
        bossLayer = gameObject.layer;
    }

    // ============================================================
    // MAIN UPDATE LOOP
    // ============================================================
    void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                CheckZone();
                break;

            case BossState.Moving:
                if (playerMouse != null)
                {
                    MoveToTarget(playerMouse.transform.position);
                }
                break;

            case BossState.Dashing:
                DashBackward();
                break;
        }

        if (health <= 1)
        {
            Destroy(bossZone);
            Debug.Log("DIE?");
        }
        else if (health <= 50)
        {
            CreateshieldAmount();
        }

        healthDisplayText.text = health.ToString();
        shieldDisplayText.text = shieldAmount.ToString();
    }

    // ============================================================
    // DETECTION / PERCEPTION
    // ============================================================
    void CheckZone()
    {
        if (damageInvulnerable) return;
        Vector2 center = transform.position;

        // Find every Collider2D inside the rectangular detection zone.
        // DetectionLayer controls which physics layers are considered. This is essentially the boss's current "perception system".
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxSize, 0f, detectionLayer);
        
        foreach (Collider2D hit in hits)
        {
            // Currently, the only object type that causes a reaction is something on the "Grabable" layer. Which is the falling box.
            if (hit.gameObject.layer == LayerMask.NameToLayer("Grabable"))
            {
                Debug.Log("Obstacle detected in zone!");
                currentState = BossState.Dashing;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // --------------------------------------------------------
        // PLAYER ENTERS BOSS TRIGGER
        // --------------------------------------------------------
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!damageInvulnerable && swissHealthScript.invulnerable == false)
            {
                AttackPlayer();
            }
        }

        // --------------------------------------------------------
        // GRABABLE OBJECT HITS BOSS
        // --------------------------------------------------------
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Grabable"))
        {
            Debug.Log("BOSS GOT HIT!");

            int remainingDamage = 25;
            Destroy(collision.gameObject);

            // Boss becomes temporarily invulnerable / pass-through
            StartCoroutine(DamageInvulnerability());

            if (shieldAmount > 0)
            {
                int shieldAmountAbsorb = Mathf.Min(shieldAmount, remainingDamage);
                shieldAmount -= shieldAmountAbsorb;
                remainingDamage -= shieldAmountAbsorb;
            }

            if (remainingDamage > 0)
            {
                health -= remainingDamage;
                health = Mathf.Max(health, 0);
            }
        }
    }

    // ============================================================
    // CONTINUOUS PLAYER COLLISION
    // ============================================================
    private void OnTriggerStay2D(Collider2D collision)
    {
        // While the player remains inside the trigger, repeatedly attempt to attack.
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!damageInvulnerable && swissHealthScript.invulnerable == false)
            {
                AttackPlayer();
            }
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

    void CreateshieldAmount()
    {
        if (!hasShield)
        {
            animator.SetBool("CreateShield", true);
            Debug.Log("Boss is creating shieldAmount.");
            shieldAmount = 100;

            StartCoroutine(WaitForshieldAmountAnimation());
        }
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

    public void MoveToTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            Vector2 moveDir = direction.normalized;
            rb.velocity = moveDir * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    // ============================================================
    // DEBUG VISUALIZATION
    // ============================================================
    void OnDrawGizmos()
    {
        Gizmos.color = boxColor;
        Gizmos.DrawCube(transform.position, boxSize);

        Color wireColor = boxColor;
        wireColor.a = 1f;
        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    IEnumerator DamageInvulnerability()
    {
        damageInvulnerable = true;
        Debug.Log("Boss was hit! Player can walk through boss.");
        // Stop boss movement immediately
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Stop AI movement/state
        currentState = BossState.Idle;
        // Ignore collision between Boss and Player
        Physics2D.IgnoreLayerCollision(bossLayer, playerLayer, true);

        // Optional: stop attack animation
        if (animator != null)
        {
            animator.SetBool("Attack", false);
        }

        // Wait for invulnerability duration
        yield return new WaitForSeconds(damageInvulnerabilityDuration);

        // Allow Boss and Player to collide again
        Physics2D.IgnoreLayerCollision(bossLayer, playerLayer, false);
        damageInvulnerable = false;
        Debug.Log("Boss can attack and collide with player again!");
    }

    IEnumerator WaitForshieldAmountAnimation()
    {
        // Wait until the animation has enough time to play
        yield return new WaitForSeconds(1f);

        animator.SetBool("CreateShield", false);  // turn off bool if needed
        hasShield= true;
        Debug.Log("shieldAmount animation finished → Boss is now Idle.");
    }
}