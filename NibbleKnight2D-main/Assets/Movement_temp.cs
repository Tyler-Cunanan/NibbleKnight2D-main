using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float axisX;
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public float jumpforce = 2;
    public LayerMask ground;
    public Transform groundCheck;

    private bool flip, _isGrounded;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        axisX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(axisX*moveSpeed, rb.velocity.y);

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded()) {
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        }
        if(Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f ) {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
    }
    
    private bool isGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);
    }
}
