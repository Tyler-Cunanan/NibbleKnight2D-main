using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField]private float speed;
    [SerializeField]private float jumpPower;
    [Header("Coyote Time")]
    [SerializeField]private float coyoteTime;
    private float coyoteCounter;
    [Header("Wall Jumping")]
    [SerializeField]private float wallJumpx;
    [SerializeField]private float wallJumpy;

    [Header("Layers")]
    [SerializeField]private LayerMask groundLayer;
    [SerializeField]private LayerMask wallLayer;
    private Rigidbody2D body;
    private CapsuleCollider2D capsuleCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    //FootStep
    private float footstepTimer;
    public float footstepDelay = 0.4f;



    [Header("Audio")]
    public List<AudioClip> audioJumpClips = new List<AudioClip>();
    public List<AudioClip> audioWalkClips = new List<AudioClip>();
    public AudioSource audioSource;
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        bool isMoving = Mathf.Abs(horizontalInput) > 0.01f;

        // Play footsteps only every few seconds
        if (isMoving && isGrounded())
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                int randomIndex = UnityEngine.Random.Range(0, audioWalkClips.Count);

                audioSource.PlayOneShot(audioWalkClips[randomIndex]);

                footstepTimer = footstepDelay;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        //Flip Player when moving left or right
        if (horizontalInput > 0.01f )
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f )
        {
            transform.localScale = new Vector3 (-1, 1, 1);
        }


        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
            jump();   

        //Adjustable jump height
        if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

        if (onWall())
        {
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (isGrounded())
            {
                coyoteCounter = coyoteTime;
            }
            else
                coyoteCounter-= Time.deltaTime;
        }
    }
    private void jump()
    {
        if (coyoteCounter <0 && !onWall() ) return;  // if coyote counter is 0 or less other codes not work
       
        if (onWall()) return;
            // WallJump();
        else
        {
            if (isGrounded())
            {
                PlayRandomJumpClip();
                body.velocity = new Vector2(body.velocity.x, jumpPower);
            }
            else
            {
                if (coyoteCounter > 0)
                    body.velocity = new Vector2(body.velocity.x, jumpPower);
            }

            coyoteCounter = 0;
        }
    }

    // private void WallJump()
    // {
    //     body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpx , wallJumpy));
    //     wallJumpCooldown = 0;
    // }

    void PlayRandomJumpClip()
    {
        if (audioJumpClips.Count == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, audioJumpClips.Count);
        audioSource.PlayOneShot(audioJumpClips[randomIndex]);
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}
