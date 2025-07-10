using System.Collections;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Moving,
        Dashing,
        Cooldown
    }

    private BossState currentState = BossState.Idle;

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

    //[Header("Optional Components")]
    //public Animator animator;
    //public AudioSource dashSound;

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
        

        Vector3 targetPosition = playerMouse.transform.position;
        // Ensure z-axis is ignored for 2D (set it to the same as the current object's z to avoid any depth issues)
        targetPosition.z = transform.position.z;

        foreach (Collider2D hit in hits)
        {
            //Step2: Add another if condition + Layer name to tell it to do something.
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player")) // Adjust layer name to match your project
            {
                Debug.Log("Enemy detected in zone!");
                FaceTarget(targetPosition);
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("Grabable"))
            {
                Debug.Log("Obstacle detected in zone!");
                StartDash();
            }
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

    void HandleCooldown()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            currentState = BossState.Idle;
        }
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        // Smoothly transition between the scales if needed
        float targetScaleX = direction.x > 0 ? 1f : -1f;

        // Optionally use Mathf.Lerp to make the transition smoother
        if (transform.localScale.x != targetScaleX)
        {
            float smoothSpeed = 5f; // Adjust the speed of flipping
            float newScaleX = Mathf.Lerp(transform.localScale.x, targetScaleX, Time.deltaTime * smoothSpeed);
            transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            // No smoothing if instant flip is preferred
            transform.localScale = new Vector3(targetScaleX, transform.localScale.y, transform.localScale.z);
        }
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