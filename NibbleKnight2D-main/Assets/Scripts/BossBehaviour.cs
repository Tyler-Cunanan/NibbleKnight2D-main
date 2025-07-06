using System.Collections;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Dashing,
        Cooldown
    }

    private BossState currentState = BossState.Idle;

    [Header("Zone Settings")]
    public Vector2 boxSize = new Vector2(5f, 5f);
    public Color boxColor = new Color(0f, 1f, 0f, 0.25f);
    public LayerMask detectionLayer;

    [Header("Dash Settings")]
    public float dashDistance = 3f;
    public float dashSpeed = 10f;
    public float dashCooldown = 2f;

    private Vector3 dashStart;
    private Vector3 dashTarget;
    private float cooldownTimer = 0f;

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

        if (hits.Length > 0)
        {
            StartDash();
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

        if (direction.x > 0)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else
            transform.localScale = new Vector3(-1f, 1f, 1f);
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