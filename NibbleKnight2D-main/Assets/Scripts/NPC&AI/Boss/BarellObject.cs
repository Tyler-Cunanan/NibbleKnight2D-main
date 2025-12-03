using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarellObject : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hook Projectile"))
        {
            rb.simulated = true;
            rb.gravityScale = 1f;
        }
    }
}
